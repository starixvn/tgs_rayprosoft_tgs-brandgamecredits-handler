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

        var publish = new StatisticalFinishPublish
        {
            ResellerId = info.ResellerId,
            ResellerCode = info.ResellerCode,
            LicenseeId = info.LicenseeId,
            LicenseeCode = info.LicenseeCode,
            BrandId = info.BrandId,
            BrandCode = info.BrandCode,
        };

        await messageSender.PublishAsync(
            subject: "game.gamewager.brand.stat.finish.publish",
            data: publish.ToByteArray(),
            cancellationToken: HttpContext.RequestAborted);
    }
}
