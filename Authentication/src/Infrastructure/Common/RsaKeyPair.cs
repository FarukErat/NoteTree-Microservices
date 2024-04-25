using Application.Interfaces.Infrastructure;

namespace Infrastructure.Common;

// TODO: consider sending PublicKey as byte[] instead of string in gRPC services
public sealed class RsaKeyPair(byte[] privateKey, string publicKey) : IRsaKeyPair
{
    public byte[] PrivateKey { get; } = privateKey;
    public string PublicKey { get; } = publicKey;
}
