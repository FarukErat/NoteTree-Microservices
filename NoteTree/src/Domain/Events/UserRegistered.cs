namespace Domain.Events;

public sealed record class UserRegisteredEvent(
    Guid UserId
);
