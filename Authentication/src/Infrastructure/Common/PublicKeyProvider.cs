using Application.Interfaces.Infrastructure;

namespace Infrastructure.Common;

public sealed class PublicKeyProvider(
    string currentKeyId,
    Dictionary<string, byte[]> keys
) : IPublicKeyProvider
{
    private readonly string? _currentKeyId = currentKeyId;
    private readonly Dictionary<string, byte[]> _keys = keys;

    public (string? keyId, byte[]? publicKey) GetCurrentPublicKey()
    {
        if (_currentKeyId == null)
        {
            return (null, null);
        }
        return (_currentKeyId, _keys.GetValueOrDefault(_currentKeyId));
    }

    public (string keyId, byte[]? publicKey) GetPublicKeyById(string keyId)
    {
        return (keyId, _keys.GetValueOrDefault(keyId));
    }
}
