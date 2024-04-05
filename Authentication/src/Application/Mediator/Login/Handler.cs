using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Enums;
using ErrorOr;
using MediatR;

namespace Application.Mediator.Login;

public sealed class LoginHandler(
    IUserReadRepository userReadRepository,
    IUserWriteRepository userWriteRepository,
    IJwtGenerator jwtGenerator,
    IPasswordHashService passwordHashService)
    : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
{
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    public async Task<ErrorOr<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Error.Validation("Username and password are required");
        }

        User? existingUser = await _userReadRepository.GetByUsernameAsync(request.Username);
        if (existingUser is null)
        {
            return Error.NotFound("User not found");
        }

        (bool verified, PasswordHashAlgorithm algorithm) = _passwordHashService.VerifyPassword(request.Password, existingUser.PasswordHash);
        if (!verified)
        {
            return Error.Conflict("Invalid password");
        }

        if (existingUser.PasswordHashAlgorithm != algorithm)
        {
            (string passwordHash, _) = _passwordHashService.HashPassword(request.Password);
            existingUser.PasswordHash = passwordHash;
            existingUser.PasswordHashAlgorithm = algorithm;
            await _userWriteRepository.UpdateAsync(existingUser);
        }

        string clientIp = request.ClientIp ?? "unknown";

        string token = _jwtGenerator.GenerateToken(
            existingUser,
            clientIp);

        return new LoginResponse(
            UserId: existingUser.Id,
            Token: token);
    }
}
