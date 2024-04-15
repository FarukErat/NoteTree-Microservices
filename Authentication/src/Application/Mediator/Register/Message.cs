namespace Application.Mediator.Register;

public sealed record class UserRegisteredMessage(
    Guid UserId);
