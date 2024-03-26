using Domain.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Infrastructure;

public interface IAuthenticationService
{
    ErrorOr<Success> Register(User user, string password);

    ErrorOr<string> Login(string username, string password);

    ErrorOr<Success> Logout(HttpContext httpContext);
}
