using ErrorOr;
using MediatR;

namespace Mocks.Mediator;

public sealed class MockHandler : IRequestHandler<MockRequest, ErrorOr<MockResponse>>
{
    public async Task<ErrorOr<MockResponse>> Handle(MockRequest request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new MockResponse(Guid.NewGuid()));
    }
}
