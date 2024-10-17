using ErrorOr;
using MediatR;

namespace Mocks.Mediator;

public sealed record class MockRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName
) : IRequest<ErrorOr<MockResponse>>;
