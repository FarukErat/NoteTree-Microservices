namespace Application.Interfaces.Infrastructure;

public interface IPublicKeyProvider
{
    (string? keyId, byte[]? publicKey) GetCurrentPublicKey();
    (string keyId, byte[]? publicKey) GetPublicKeyById(string keyId);
}
