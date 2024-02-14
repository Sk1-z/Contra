using System.Security.Cryptography;

namespace Contra.Security;

interface IEncryptor
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

    public byte[] Encrypt(byte[] msg, byte[] payload);
}

// class KeyEncryptor : IEncryptor

// class PasswordEncryptor : IEncryptor

// class PseudoEncryptor : IEncryptor

