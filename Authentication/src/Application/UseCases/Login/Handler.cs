using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Entities;
using ErrorOr;
using MediatR;

namespace Application.UseCases.Login;

public sealed class LoginHandler(
    IUserReadRepository userReadRepository,
    IUserWriteRepository userWriteRepository,
    IJwtGenerator jwtGenerator,
    IPasswordHasherFactory passwordHasherFactory
) : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
{
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    private readonly IPasswordHasherFactory _passwordHasherFactory = passwordHasherFactory;

    public async Task<ErrorOr<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        // TODO: do not validate the DTO, since it is already validate in the pipeline
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Error.Validation(description: "Username and password are required");
        }

        User? existingUser = await _userReadRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser is null)
        {
            return Error.NotFound(description: "User not found");
        }

        IPasswordHasher? passwordHasher = _passwordHasherFactory.GetPasswordHasher(existingUser.PasswordHashAlgorithm);
        if (passwordHasher is null)
        {
            return Error.Unexpected(description: "Password hash algorithm not supported");
        }

        if (!passwordHasher.VerifyPassword(request.Password, existingUser.PasswordHash))
        {
            return Error.Conflict(description: "Invalid password");
        }

        if (existingUser.PasswordHashAlgorithm != Configurations.PasswordHashAlgorithm)
        {
            passwordHasher = _passwordHasherFactory.GetPasswordHasher(Configurations.PasswordHashAlgorithm);
            if (passwordHasher is null)
            {
                return Error.Unexpected(description: "Password hash algorithm not supported");
            }
            string passwordHash = passwordHasher.HashPassword(request.Password);
            existingUser.PasswordHash = passwordHash;
            existingUser.PasswordHashAlgorithm = Configurations.PasswordHashAlgorithm;
            await _userWriteRepository.UpdateAsync(existingUser, cancellationToken);
        }

        string token = _jwtGenerator.GenerateRefreshToken(
            userId: existingUser.Id,
            audience: request.ClientIp);

        return new LoginResponse(
            UserId: existingUser.Id,
            RefreshToken: token);
    }
}
