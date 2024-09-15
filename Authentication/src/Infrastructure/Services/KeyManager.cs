using System.Security.Cryptography;

namespace Infrastructure.Services;

// TODO: use a storage service instead of manually handling the files (sqlite?)
public static class KeyManager
{

    public static string CurrentDir => Directory.GetCurrentDirectory();
    public static string ParentDir => Directory.GetParent(CurrentDir)?.FullName ?? CurrentDir;
    public static string KeysFolder => Path.Combine(ParentDir, "Keys");
    public static string CurrentPublicKeyIdFilePath => Path.Combine(KeysFolder, "current_public_key_id.pem");

    public static string PrivateKeyFileName => "private_key.pem";
    public static string PublicKeyFileName => "public_key.pem";

    private static string KeyFolderPath(string keyId) => Path.Combine(KeysFolder, keyId);
    private static string PrivateKeyFilePath(string keyId) => Path.Combine(KeyFolderPath(keyId), PrivateKeyFileName);
    private static string PublicKeyFilePath(string keyId) => Path.Combine(KeyFolderPath(keyId), PublicKeyFileName);

    public static (string keyId, byte[] privateKey, byte[] publicKey) LoadOrCreateRsaKeyPair()
    {
        if (!Directory.Exists(KeysFolder))
        {
            Directory.CreateDirectory(KeysFolder);
        }

        string currentKeyId = GetCurrentKeyId() ?? string.Empty;
        bool currentKeyIdExists = !string.IsNullOrEmpty(currentKeyId);
        bool currentKeyFolderExists = currentKeyIdExists
            && Directory.Exists(KeyFolderPath(currentKeyId));
        bool currentKeyFilesExist = currentKeyFolderExists
            && File.Exists(PrivateKeyFilePath(currentKeyId))
            && File.Exists(PublicKeyFilePath(currentKeyId));

        if (!currentKeyFilesExist)
        {
            using RSA rsa = RSA.Create();
            byte[] privateKey = rsa.ExportRSAPrivateKey();
            byte[] publicKey = rsa.ExportRSAPublicKey();

            string newKeyId = Guid.NewGuid().ToString();
            string newKeyFolderPath = KeyFolderPath(newKeyId);
            Directory.CreateDirectory(newKeyFolderPath);

            string privateKeyFilePath = PrivateKeyFilePath(newKeyId);
            string publicKeyFilePath = PublicKeyFilePath(newKeyId);

            File.WriteAllBytes(privateKeyFilePath, privateKey);
            File.WriteAllBytes(publicKeyFilePath, publicKey);

            UpdateCurrentKeyId(newKeyId);

            return (newKeyId, privateKey, publicKey);
        }
        else
        {
            byte[] privateKey = File.ReadAllBytes(PrivateKeyFilePath(currentKeyId));
            byte[] publicKey = File.ReadAllBytes(PublicKeyFilePath(currentKeyId));

            return (currentKeyId, privateKey, publicKey);
        }
    }

    public static string? GetCurrentKeyId()
    {
        string latestKeyFilePath = CurrentPublicKeyIdFilePath;
        if (File.Exists(latestKeyFilePath))
        {
            return File.ReadAllText(latestKeyFilePath).Trim();
        }
        return null;
    }

    public static void UpdateCurrentKeyId(string keyId)
    {
        string latestKeyFilePath = CurrentPublicKeyIdFilePath;
        File.WriteAllText(latestKeyFilePath, keyId);
    }

    public static Dictionary<string, byte[]> GetPublicKeys()
    {
        Dictionary<string, byte[]> keys = [];
        foreach (string keyDir in Directory.GetDirectories(KeysFolder))
        {
            string keyId = Path.GetFileName(keyDir);
            byte[] publicKey = File.ReadAllBytes(Path.Combine(keyDir, PublicKeyFileName));
            keys.Add(keyId, publicKey);
        }
        return keys;
    }
}
