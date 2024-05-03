using ErrorOr;
using Grpc.Net.Client;
using static Infrastructure.Proto.Authentication;

namespace Infrastructure.Services;

public sealed class GetVerificationKeyService
{
    private readonly GrpcChannel _channel;
    private readonly AuthenticationClient _client;

    public GetVerificationKeyService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.ConnectionStrings.AuthenticationUrl);
        _client = new AuthenticationClient(_channel);
    }

    public byte[]? GetVerificationKey(string keyId)
    {
        try
        {
            // TODO: consider using GetVerificationKeyAsync instead of GetVerificationKey
            Proto.VerificationKeyResponse response = _client.GetVerificationKey(
                new Proto.VerificationKeyRequest()
                {
                    KeyId = keyId
                });
            return response.Key.ToByteArray();
        }
        catch
        {
            return null;
        }
    }
}
