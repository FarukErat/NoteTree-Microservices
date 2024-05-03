namespace Application.UseCases.GetVerificationKey;

public sealed record class GetVerificationKeyResponse(
    string? KeyId,
    byte[]? VerificationKey);
