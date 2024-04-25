using Application;
using Infrastructure;
using Persistence;
using Presentation;
using Presentation.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPersistence()
    .AddPresentation();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<NoteTreeService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
