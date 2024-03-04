using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Messages;
using Tgs.GameWagers.Stat.Daily.Common;
using Tgs.MessageQueue.Nats.Serializers;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
            fluentOptions.UseMessagePack();
        });
    });

// Add services to the container.

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
    foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);
});

builder.Services.AddBrandGameCredits(
    (sp, dbBuilder) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        _ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"));
    });

builder.Services
    .AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
    {
        Url = builder.Configuration.GetConnectionString("Nats")!,
        SerializerRegistry = TgsNatsSerializerRegistration.Default
    }))
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
    .ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
    .AddHandler<StatCloseMessageHandler>(GameWagersStatDailyNatsConst.StatDailyCloseFinish, typeof(StatCloseMessageHandler).FullName!.ToLower())
    .AddHandler<CreateMessageHandler>(BrandGameCreditsHandlerNatsConst.Create, BrandGameCreditsHandlerNatsConst.Create)
    .AddHandler<InitialMessageHandler>(BrandGameCreditsHandlerNatsConst.Initial, BrandGameCreditsHandlerNatsConst.Initial)
    .AddHandler<CreditMessageHandler>(BrandGameCreditsHandlerNatsConst.Credit, BrandGameCreditsHandlerNatsConst.Credit)
    .AddHandler<DebitMessageHandler>(BrandGameCreditsHandlerNatsConst.Debit, BrandGameCreditsHandlerNatsConst.Debit)
    .AddHandler<ProfitMessageHandler>(BrandGameCreditsHandlerNatsConst.Profit, BrandGameCreditsHandlerNatsConst.Profit)
    .AddHandler<SettlementMessageHandler>(BrandGameCreditsHandlerNatsConst.Settlement, BrandGameCreditsHandlerNatsConst.Settlement))
    .ClearAllExchanges()
    .AddNatsGlobPatternExchange(GameWagersStatDailyNatsConst.StatDailyCloseFinish)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Create)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Initial)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Credit)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Debit)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Profit)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Settlement)
    .AddNatsGlobPatternExchange(BrandGameCreditsHandlerNatsConst.Update);

builder.Services
    .AddOpenTelemetry()
    .WithTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddSource("Valhalla.*")
        .AddAspNetCoreInstrumentation(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/profiler/")
            && !httpContext.Request.Path.StartsWithSegments("/hangfire")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
        .AddHttpClientInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddGrpcCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
        .AddOtlpExporter())
    .WithMetrics(meterBuiler => meterBuiler
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddEnvironmentVariableDetector())
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter())
    .Services
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();
