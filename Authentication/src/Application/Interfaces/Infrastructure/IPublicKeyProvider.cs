namespace Application.Interfaces.Infrastructure;

public interface IPublicKeyProvider
{
    (string? keyId, byte[]? publicKey) GetCurrentPublicKey();
    byte[]? GetPublicKeyById(string keyId);
}
