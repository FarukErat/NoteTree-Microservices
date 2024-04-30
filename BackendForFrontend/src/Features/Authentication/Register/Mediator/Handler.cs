using Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Features.Authentication.Register;

public sealed class RegisterHandler(
    RegisterService client
) : IRequestHandler<RegisterRequest, ErrorOr<RegisterResponse>>
{
    private readonly RegisterService _client = client;
    public async Task<ErrorOr<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        ErrorOr<Guid> result = await _client.Register(
            username: request.Username,
            password: request.Password,
            email: request.Email,
            firstName: request.FirstName,
            lastName: request.LastName
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
