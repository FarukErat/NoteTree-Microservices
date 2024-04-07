namespace Application.Mediator.Register;

using MediatR;
using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Enums;
using ErrorOr;

public sealed class RegisterHandler(
    IUserReadRepository userReadRepository,
    IUserWriteRepository userWriteRepository,
    IPasswordHashService passwordHashService)
    : IRequestHandler<RegisterRequest, ErrorOr<RegisterResponse>>
{
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    public async Task<ErrorOr<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName))
        {
            return Error.Validation("Username, password, email, first name, and last name are required");
        }

        User? existingUser = await _userReadRepository.GetByUsernameAsync(request.Username);
        if (existingUser is not null)
        {
            return Error.Conflict("Username already exists");
        }
        existingUser = await _userReadRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Error.Conflict("Email already exists");
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

        return new RegisterResponse(
            UserId: newUser.Id);
    }
}