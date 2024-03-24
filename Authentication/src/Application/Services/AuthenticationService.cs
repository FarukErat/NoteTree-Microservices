using Grpc.Core;
using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public sealed class AuthenticationService(
    IUserWriteRepository userWriteRepository,
    IUserReadRepository userReadRepository,
    IJwtGenerator jwtGenerator,
    IPasswordHashService passwordHashService,
    IRsaKeyPair rsaKeyPair)
    : Authentication.AuthenticationBase
{
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IRsaKeyPair _rsaKeyPair = rsaKeyPair;

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName))
        {
            return new RegisterResponse
            {
                Success = false,
                Message = "Username, password, email, first name, and last name are required",
            };
        }

        User? existingUser = await _userReadRepository.GetByUsernameAsync(request.Username);
        if (existingUser is not null)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = "Username already exists",
            };
        }
        existingUser = await _userReadRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = "Email already exists",
            };
        }

        (string passwordHash, PasswordHashAlgorithm algorithm) = _passwordHashService.HashPassword(request.Password);
        User newUser = new()
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = Role.User,
            PasswordHash = passwordHash,
            PasswordHashAlgorithm = algorithm
        };

        await _userWriteRepository.CreateAsync(newUser);

        return new RegisterResponse
        {
            Success = true,
            Message = "User registered successfully"
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Username and password are required",
                Token = ""
            };
        }

        User? existingUser = await _userReadRepository.GetByUsernameAsync(request.Username);
        if (existingUser is null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "User not found",
                Token = ""
            };
        }

        (bool verified, PasswordHashAlgorithm algorithm) = _passwordHashService.VerifyPassword(request.Password, existingUser.PasswordHash);
        if (!verified)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid password",
                Token = ""
            };
        }

        if (existingUser.PasswordHashAlgorithm != algorithm)
        {
            (string passwordHash, _) = _passwordHashService.HashPassword(request.Password);
            existingUser.PasswordHash = passwordHash;
            existingUser.PasswordHashAlgorithm = algorithm;
            await _userWriteRepository.UpdateAsync(existingUser);
        }

        string clientIp = context.GetHttpContext().Connection.RemoteIpAddress?.ToString() ?? "unknown";
        // string clientPort = context.GetHttpContext().Connection.RemotePort.ToString();

        string token = _jwtGenerator.GenerateToken(
            existingUser,
            clientIp);

        return new LoginResponse
        {
            Success = true,
            Message = "User logged in successfully",
            Token = token
        };
    }

    public override Task<VerificationKeyResponse> GetVerificationKey(VerificationKeyRequest request, ServerCallContext context)
    {
        return Task.FromResult(new VerificationKeyResponse
        {
            Key = Convert.ToBase64String(_rsaKeyPair.PublicKey)
        });
    }
}
