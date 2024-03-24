namespace Application.Interfaces.Infrastructure;

public interface IRsaKeyPair
{
    byte[] PrivateKey { get; }
    byte[] PublicKey { get; }
}
