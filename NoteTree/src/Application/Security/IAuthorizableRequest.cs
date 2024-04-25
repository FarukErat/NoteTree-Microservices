using MediatR;

namespace Security;

public interface IAuthorizeableRequest<T>
    : IRequest<T>
{
    string Jwt { get; }
}
