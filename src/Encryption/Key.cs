using System.Security.Cryptography;

namespace Contra.Security;

public class Key
{
    private byte[] _key = new byte[Size.Key(Size.Unit.Byte)];

    public Key()
    {
        RandomNumberGenerator.Create().GetBytes(_key);
    }

    public static Key? FromString(string key)
    {
        Key newKey = new();

        try
        {
            for (int i = 0; i < Size.Key(Size.Unit.Byte); i++) newKey._key[i] = Convert.ToByte(key[(2 * i)..((2 * i) + 2)], 16);
            return newKey;
        }
        catch
        {
            return null;
        }
    }

    public Byte[] Bytes()
    {
        return _key;
    }

    override public string ToString()
    {
        string acc = "";
        for (int i = 0; i < Size.Key(Size.Unit.Byte); i++) acc += _key[i].ToString("X2");
        return acc;
    }
}
