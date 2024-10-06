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

    public static async Task<TResponse> Log<TRequest, TResponse>(
        IRequest<TResponse> request,
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr, new()
    {
        LoggingBehavior<TRequest, TResponse> loggingBehavior = new(logger);

        return await loggingBehavior.Handle(
            (TRequest)request,
            () => Task.FromResult(new TResponse()),
            CancellationToken.None);
    }
}
