using Application.Interfaces.Infrastructure;
using ErrorOr;
using MediatR;

namespace Application.Mediator.Register;

public sealed class RegisterHandler(
    IAuthenticationService client)
    : IRequestHandler<RegisterRequest, ErrorOr<RegisterResponse>>
{
    private readonly IAuthenticationService _client = client;
    public async Task<ErrorOr<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        ErrorOr<Guid> result = await _client.Register(
            request.Username,
            request.Password,
            request.Email,
            request.FirstName,
            request.LastName
        );

        if (result.IsError)
        {
            return result.FirstError;
        }

        return new RegisterResponse(
            UserId: result.Value
        );
    }
}
