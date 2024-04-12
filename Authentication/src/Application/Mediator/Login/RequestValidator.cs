using FluentValidation;

namespace Application.Mediator.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters")
            .Matches("^[a-zA-Z0-9]*$")
            .WithMessage("Username must contain only letters and numbers");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters")
            .MaximumLength(20)
            .WithMessage("Password must not exceed 20 characters");
    }
}
