using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NATS.Client.JetStream;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.BrandGameCredits.Handler.ViewModels;
using Tgs.ServiceUtility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tgs.BrandGameCredits.Handler.Controllers;

[Route(template: "[controller]")]
[ApiController]
public class CreateController : ControllerBase
{
    /// <summary>
    /// Posts the asynchronous.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "brandCode": "GPI",
    ///             "MonthTime": "2023-12-01T00:00:00+08:00"
    ///         }
    /// </remarks>
    /// <exception cref="System.NullReferenceException">This brand does not exist.{source.BrandCode}</exception>
    [HttpPost]
    public async ValueTask PostAsync(
        [FromServices] IBrandRepository repository,
        [FromServices] ICommandService<PublishCommand> service,
        CreateViewModel source)
    {
        var info = await repository.GetAsync(
            code: source.BrandCode,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? throw new NullReferenceException($"This brand does not exist.{source.BrandCode}");

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Create,
                ResellerId: info.ResellerId,
                ResellerCode: info.ResellerCode,
                LicenseeId: info.LicenseeId,
                LicenseeCode: info.LicenseeCode,
                BrandId: info.BrandId,
                BrandCode: info.BrandCode,
                CurrentTime: source.MonthTime),
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Posts all asynchronous.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///         {
    ///             "MonthTime": "2023-12-01T00:00:00+08:00"
    ///         }
    /// </remarks>
    [HttpPost("all")]
    public async ValueTask PostAllAsync(
        [FromServices] IBrandRepository repository,
        [FromServices] ICommandService<PublishCommand> service,
        CreateMonthViewModel source)
    {
        var infos = await repository.QueryAsync(
            cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<BrandInfo>();

        if (infos.Any())
        {
            foreach (var info in infos)
            {
                await service.ExecuteAsync(
                    command: new PublishCommand(
                        Subject: BrandGameCreditsHandlerNatsConst.Create,
                        ResellerId: info.ResellerId,
                        ResellerCode: info.ResellerCode,
                        LicenseeId: info.LicenseeId,
                        LicenseeCode: info.LicenseeCode,
                        BrandId: info.BrandId,
                        BrandCode: info.BrandCode,
                        CurrentTime: source.MonthTime),
                    cancellationToken: HttpContext.RequestAborted)
                    .ConfigureAwait(false);
            }
        }
    }
}
