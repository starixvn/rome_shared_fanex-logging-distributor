using System.Security.Cryptography;
using System.Text;

namespace Fanex.Logging.Distributor.Helpers;

public static class EasyNetQConnectionStringCryptography
{
    private const int KeyLength = 32;
    private const int IVLength = 16;
    private const int RandomNumber = 223;
    private const string Key = "QWERT!@#$%12345";
    private const string IV = "%$#@!TREWQ54321";

    public static string Encrypt(string connectionString)
    {
        var configurationConfig = GetConfig(connectionString);
        byte[] encrypted = null;

        using (var AES = Aes.Create())
        {
            AES.Key = ConvertToKey(Key, KeyLength);
            AES.IV = ConvertToKey(IV, IVLength);

            // Encrypt the string to an array of bytes.
            encrypted = EncryptStringToBytes(configurationConfig["password"], AES.Key, AES.IV);
        }

        configurationConfig["password"] = Convert.ToBase64String(encrypted);
        return MergeConnectionConfig(configurationConfig);
    }

    public static string Decrypt(string encryptedConnectionString)
    {
        var configurationConfig = GetConfig(encryptedConnectionString);
        var decrypted = string.Empty;

        using (var AES = Aes.Create())
        {
            AES.Key = ConvertToKey(Key, KeyLength);
            AES.IV = ConvertToKey(IV, IVLength);

            // Decrypt the bytes to a string.
            decrypted = DecryptStringFromBytes(Convert.FromBase64String(configurationConfig["password"]), AES.Key, AES.IV);
        }

        return decrypted;
    }

    public static string DecryptConnectionString(string connectionString)
    {
        var configurationConfig = GetConfig(connectionString);

        configurationConfig["password"] = Decrypt(connectionString);

        return MergeConnectionConfig(configurationConfig);
    }

    private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("connectionString");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (var AES = Aes.Create())
        {
            AES.Key = Key;
            AES.IV = IV;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = AES.CreateEncryptor(AES.Key, AES.IV);

            // Create the streams used for encryption.
            using MemoryStream msEncrypt = new();
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (var AES = Aes.Create())
        {
            AES.Key = Key;
            AES.IV = IV;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = AES.CreateDecryptor(AES.Key, AES.IV);

            // Create the streams used for decryption.
            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            plaintext = srDecrypt.ReadToEnd();
        }

        return plaintext;
    }

    private static byte[] ConvertToKey(string plainText, int keyLegnth)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        byte[] result = new byte[keyLegnth];

        if (bytes.Length > keyLegnth)
        {
            for (int i = 0; i < keyLegnth; i++)
            {
                result[i] = bytes[i];
            }
        }
        else if (bytes.Length < keyLegnth)
        {
            bytes.CopyTo(result, 0);

            for (int i = bytes.Length; i < keyLegnth; i++)
            {
                result[i] = RandomNumber;
            }
        }

        return result;
    }

    private static Dictionary<string, string> GetConfig(string connectionString)
    {
        Dictionary<string, string> connectionStringConfig = new();
        var configParsed = connectionString.Split(';');

        foreach (var configItem in configParsed)
        {
            if (configItem.Contains("password"))
            {
                connectionStringConfig.Add("password", configItem.Replace("password=", string.Empty));
            }
            else
            {
                var configData = configItem.Split('=');
                connectionStringConfig.Add(configData[0], configData[1]);
            }
        }

        return connectionStringConfig;
    }

    private static string MergeConnectionConfig(Dictionary<string, string> connectionStringConfig)
    {
        var connectionString = string.Empty;
        var index = 0;
        var length = connectionStringConfig.Count;
        foreach (var configItem in connectionStringConfig)
        {
            connectionString += configItem.Key + "=" + configItem.Value;
            if (index < length - 1)
            {
                connectionString += ";";
            }
            index++;
        }

        return connectionString;
    }
}

