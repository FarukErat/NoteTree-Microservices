#region service configuration

using Common.Interfaces;
using Infrastructure.Common;
using Common.Behaviors;
using Common.Filters;
using FluentValidation;
using Features.Authentication.Login;
using Features.Authentication.Register;
using Serilog;
using Features;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
    options.Filters.Add<ExceptionHandlerFilterAttribute>());

// Register services
builder.Services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddFeatures();
builder.Services.AddSingleton<ICacheService, CacheService>();

// Serilog
Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Information()
   .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
