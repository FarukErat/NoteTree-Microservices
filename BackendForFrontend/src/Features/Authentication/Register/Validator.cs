using FluentValidation;

namespace Features.Authentication.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(6)
            .WithMessage("Username must be at least 6 characters.")
            .MaximumLength(20)
            .WithMessage("Username must be at most 20 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.")
            .MaximumLength(20)
            .WithMessage("Password must be at most 20 characters.");
    }
}
