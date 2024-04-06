namespace Application.Interfaces.Infrastructure;

public interface IRsaKeyPair
{
    byte[] PrivateKey { get; }
    string PublicKey { get; }
}
