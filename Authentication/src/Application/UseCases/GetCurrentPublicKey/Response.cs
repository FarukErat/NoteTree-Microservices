namespace Application.UseCases.GetCurrentPublicKey;

public sealed record class GetCurrentPublicKeyResponse(
    string? KeyId,
    byte[]? PublicKey
);
