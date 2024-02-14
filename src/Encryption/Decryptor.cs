using System.Security.Cryptography;

namespace Contra.Security;

interface IDecryptor
{
    private Aes? _managedAES
    {
        get => _managedAES;
        set => _managedAES = value;
    }

    private Rfc2898DeriveBytes? _keyDeriver
    {
        get => _keyDeriver;
        set => _keyDeriver = value;
    }

    private Key? _key
    {
        get => _key;
        set => _key = value;
    }

    private byte[]? _salt
    {
        get => _salt;
        set => _salt = value;
    }

    public byte[] Decrypt(byte[] msg, byte[] payload);
}

// class KeyDecryptor : IDecryptor

// class PasswordDecryptor : IDecryptor

// class PseudoDecryptor : IDecryptor

