using Google.Protobuf;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.JetStream;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Protos;
using Tgs.BrandGameCredits.Handler.ViewModels;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Tgs.GameWagers.Stat.Daily.Common;
using Tgs.GameWagers.Stat.Daily.Common.Protos;
using System.Timers;
using Google.Protobuf.WellKnownTypes;

namespace Tgs.BrandGameCredits.Handler.Controllers;

[Route(template: "[controller]")]
[ApiController]
public class StatCloseController : ControllerBase
{
    /// <summary>
    /// Posts the asynchronous.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="messageSender">The message sender.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "brandCode": "GPI"
    ///         }
    /// </remarks>
    /// <exception cref="System.NullReferenceException">This brand does not exist.{source.BrandCode}</exception>
    [HttpPost]
    public async ValueTask PostAsync(
        [FromServices] IBrandRepository repository,
        [FromServices] IMessageSender messageSender,
        StatCloseViewModel source)
    {
        var info = await repository.GetAsync(
            code: source.BrandCode,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? throw new NullReferenceException($"This brand does not exist.{source.BrandCode}");

        var currentTime = DateTimeOffset.UtcNow;
        var utc8 = currentTime.ToOffset(TimeSpan.FromHours(+8));
        var startOn = new DateTimeOffset(utc8.Year, utc8.Month, 1, 00, 00, 00, utc8.Offset);
        var endOn = startOn.AddMonths(1);

        var publish = new GameWagersStatDailyFinishMessage
        {
            ResellerId = info.ResellerId,
            ResellerCode = info.ResellerCode,
            LicenseeId = info.LicenseeId,
            LicenseeCode = info.LicenseeCode,
            BrandId = info.BrandId,
            BrandCode = info.BrandCode,
            StartOn = startOn.ToTimestamp(),
            EndOn = endOn.ToTimestamp()
        };

        await messageSender.PublishAsync(
            subject: GameWagersStatDailyNatsConst.StatDailyCloseFinish,
            data: publish.ToByteArray(),
            cancellationToken: HttpContext.RequestAborted);
    }
}
