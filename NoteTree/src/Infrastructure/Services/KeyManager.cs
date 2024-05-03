using System.Security.Cryptography;

namespace Infrastructure.Services;

public static class KeyManager
{
    public static class Directories
    {
        public static string CurrentDir => Directory.GetCurrentDirectory();
        public static string ParentDir => Directory.GetParent(CurrentDir)?.FullName ?? CurrentDir;
        public static string KeyFolder => Path.Combine(ParentDir, "Keys");
    }

    public static void SaveKey(string keyId, byte[] key)
    {
        if (!Directory.Exists(Directories.KeyFolder))
        {
            Directory.CreateDirectory(Directories.KeyFolder);
        }

        string keyFilePath = Path.Combine(Directories.KeyFolder, keyId);
        File.WriteAllBytes(keyFilePath, key);
    }

    public static byte[]? GetKey(string keyId)
    {
        string keyFilePath = Path.Combine(Directories.KeyFolder, keyId);
        bool fileExists = File.Exists(keyFilePath);
        if (!fileExists)
        {
            return null;
        }

        return File.ReadAllBytes(keyFilePath);
    }
}
