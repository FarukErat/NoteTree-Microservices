using Application.Interfaces.Infrastructure;

namespace Infrastructure.Common;

public sealed class RsaKeyPair(byte[] privateKey, string publicKey) : IRsaKeyPair
{
    public byte[] PrivateKey { get; } = privateKey;
    public string PublicKey { get; } = publicKey;
}
