using ErrorOr;

namespace Common.Interfaces;

public interface IAuthenticationService
{
    Task<ErrorOr<(Guid UserId, string Token)>> Login(string username, string password);
    Task<ErrorOr<Guid>> Register(
        string username,
        string password,
        string email,
        string firstName,
        string lastName);
}
