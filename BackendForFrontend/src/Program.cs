#region service configuration

using Infrastructure.Common;
using Common.Behaviors;
using Common.Filters;
using FluentValidation;
using Features.Authentication.Login;
using Features.Authentication.Register;
using Serilog;
using Features;
using Common.Services.TokenExchangeService;
using Common.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// API
// TODO: provide OPTIONS and HEAD methods for all endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
    options.Filters.Add<ExceptionHandlerFilterAttribute>());

// Register services
builder.Services.AddFeatures();
builder.Services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<TokenExchangeService>();
builder.Services.AddSingleton<TokenHandler>();

// Serilog
Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Information()
   .WriteTo.Console()
   .CreateLogger();
builder.Host.UseSerilog();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

#endregion

WebApplication app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
