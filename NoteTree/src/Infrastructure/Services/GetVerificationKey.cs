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

    public async Task<byte[]?> GetPublicKeyByKeyId(string keyId)
    {
        try
        {
            // TODO: consider using GetVerificationKeyAsync instead of GetVerificationKey
            Proto.GetPublicKeyByKeyIdResponse response = await _client.GetPublicKeyByKeyIdAsync(
                new Proto.GetPublicKeyByKeyIdRequest()
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

    public (string? keyId, byte[]? publicKey) GetCurrentPublicKey()
    {
        try
        {
            Proto.GetCurrentPublicKeyResponse response = _client.GetCurrentPublicKey(new Proto.GetCurrentPublicKeyRequest());
            return (response.KeyId, response.Key.ToByteArray());
        }
        catch
        {
            return (null, null);
        }
    }
}
