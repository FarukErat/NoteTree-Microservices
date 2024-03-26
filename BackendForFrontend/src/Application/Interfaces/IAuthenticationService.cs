using Application.Dtos;
using Domain.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Infrastructure;

public interface IAuthenticationService
{
    ErrorOr<Success> Register(RegisterRequest registerRequest);

    ErrorOr<string> Login(LoginRequest loginRequest);

    ErrorOr<Success> Logout(HttpContext httpContext);
}
