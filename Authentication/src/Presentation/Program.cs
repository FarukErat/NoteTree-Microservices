using Application;
using Presentation.Services;
using Infrastructure;
using Persistence;
using Presentation;
using Serilog;
using Serilog.Enrichers.Sensitive;

// TODO: check whether multiple instances of the same type of service can be consumed by the same client
// TODO: check how vertically scalable the system is

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// TODO: move logging logic to infrastructure layer
Log.Logger = new LoggerConfiguration()
    .Enrich.WithSensitiveDataMasking(options =>
    {
        options.MaskProperties.Add("password");
        options.MaskProperties.Add("token");
        options.MaskProperties.Add("refreshToken");
        options.MaskProperties.Add("accessToken");
    })
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();
builder.Host.UseSerilog();

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPersistence()
    .AddPresentation();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthenticationService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
