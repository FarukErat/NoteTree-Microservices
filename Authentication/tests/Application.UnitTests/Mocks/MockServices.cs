using Application;
using Application.Behaviors;
using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using ErrorOr;
using FluentValidation;
using MediatR;
using NSubstitute;

namespace Mocks;

public static class MockServices
{
    public static readonly ISender Sender;

    static MockServices()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        // register services
        builder.Services.AddSingleton(Substitute.For<IUserReadRepository>());
        builder.Services.AddSingleton(Substitute.For<IUserWriteRepository>());
        builder.Services.AddSingleton(Substitute.For<IPasswordHasherFactory>());
        builder.Services.AddSingleton(Substitute.For<IMessageBroker>());
        builder.Services.AddSingleton(Substitute.For<IJwtGenerator>());

        // register pipeline
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        // register MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        Sender = builder.Services.BuildServiceProvider().GetRequiredService<ISender>();
    }

    public static async Task<TResponse> Validate<TRequest, TResponse>(
        IRequest<TResponse> request,
        IEnumerable<IValidator<TRequest>> validators)
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr, new()
    {
        ValidationBehavior<TRequest, TResponse> validationBehavior = new(validators);

        return await validationBehavior.Handle(
            (TRequest)request,
            () => Task.FromResult(new TResponse()),
            CancellationToken.None);
    }
}
