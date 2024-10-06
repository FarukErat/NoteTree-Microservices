using Application.Behaviors;
using ErrorOr;
using FluentValidation;
using MediatR;

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

        static Task<TResponse> next() => Task.FromResult(new TResponse());

        return await validationBehavior.Handle(
            (TRequest)request,
            next,
            CancellationToken.None);
    }
}
