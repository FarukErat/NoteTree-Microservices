using Application.Interfaces.Infrastructure;

namespace Infrastructure.Common;

public sealed class RsaKeyPair(byte[] privateKey, byte[] publicKey) : IRsaKeyPair
{
    public byte[] PrivateKey { get; } = privateKey;
    public byte[] PublicKey { get; } = publicKey;
}
