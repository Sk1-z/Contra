using System.Security.Cryptography;

namespace Contra.Security;

public class Key
{
    private byte[] _key;

    public Key()
    {
        _key = new byte[Size.Key(Size.Unit.BYTE)];
        RandomNumberGenerator.Create().GetBytes(_key);
    }

    public Key(string key)
    {
        _key = new byte[Size.Key(Size.Unit.BYTE)];

        try
        {
            for (int i = 0; i < Size.Key(Size.Unit.BYTE); i++) _key[i] = Convert.ToByte(key[(2 * i)..((2 * i) + 2)], 16);
        }
        catch
        {
            _key = new Key()._key;
        }
    }

    public override string ToString()
    {
        string acc = "";
        for (int i = 0; i < Size.Key(Size.Unit.BYTE); i++) acc += _key[i].ToString("X2");
        return acc;
    }
}
