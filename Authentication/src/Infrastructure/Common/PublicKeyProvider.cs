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

    public byte[]? GetPublicKeyById(string keyId)
    {
        return _keys.GetValueOrDefault(keyId);
    }
}
