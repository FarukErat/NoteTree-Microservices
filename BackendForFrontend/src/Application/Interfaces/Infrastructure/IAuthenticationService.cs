using Application.Dtos;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Infrastructure;

public interface IAuthenticationService
{
    ErrorOr<RegisterResponse> Register(RegisterRequest registerRequest);

    Task<ErrorOr<LoginResponse>> Login(LoginRequest loginRequest, HttpContext httpContext);

    ErrorOr<Success> Logout(HttpContext httpContext);
}
