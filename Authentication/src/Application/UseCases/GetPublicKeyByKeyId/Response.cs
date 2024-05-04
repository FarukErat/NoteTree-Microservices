namespace Application.UseCases.GetPublicKeyByKeyId;

public sealed record class GetPublicKeyByKeyIdResponse(
    byte[]? PublicKey
);
