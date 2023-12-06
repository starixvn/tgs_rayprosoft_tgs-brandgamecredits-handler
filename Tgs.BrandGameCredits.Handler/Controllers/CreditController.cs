using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.JetStream;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.ViewModels;
using Tgs.ServiceUtility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tgs.BrandGameCredits.Handler.Controllers;

[Route(template: "[controller]")]
[ApiController]
public class CreditController : ControllerBase
{
    /// <summary>
    /// Posts the asynchronous.
    /// </summary>
    /// <param name="systemClock">The system clock.</param>
    /// <param name="repository">The repository.</param>
    /// <param name="service">The service.</param>
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
        [FromServices] ISystemClock systemClock,
        [FromServices] IBrandRepository repository,
        [FromServices] ICommandService<PublishCommand> service,
        CreditViewModel source)
    {
        var info = await repository.GetAsync(
            code: source.BrandCode,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? throw new NullReferenceException($"This brand does not exist.{source.BrandCode}");

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Credit,
                ResellerId: info.ResellerId,
                ResellerCode: info.ResellerCode,
                LicenseeId: info.LicenseeId,
                LicenseeCode: info.LicenseeCode,
                BrandId: info.BrandId,
                BrandCode: info.BrandCode,
                CurrentTime: systemClock.CurrentTime),
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }
}
