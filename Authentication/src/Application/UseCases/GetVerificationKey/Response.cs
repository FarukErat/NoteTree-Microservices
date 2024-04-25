namespace Application.UseCases.GetVerificationKey;

public sealed record class GetVerificationKeyResponse(
    byte[] VerificationKey);
