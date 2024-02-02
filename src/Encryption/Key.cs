using System.Security.Cryptography;

namespace Contra.Encryption;

public class Key
{
    private byte[] _Key;

    public Key()
    {
        RandomNumberGenerator random = RandomNumberGenerator.Create();

    }

    public Key(string key)
    {

    }

    public Key(byte[] key)
    {

    }

    public string ToString()
    {
        return "";
    }
}
