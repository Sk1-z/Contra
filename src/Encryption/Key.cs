using System.Security.Cryptography;

namespace Contra.Encryption;

public class Key
{
    private byte[] _Key;

    public Key()
    {
        _Key = new byte[Size.Key(Size.Unit.BYTE)];
        RandomNumberGenerator.Create().GetBytes(_Key);
    }

    public Key(string key)
    {
        _Key = new byte[Size.Key(Size.Unit.BYTE)];

        try
        {
            for (int i = 0; i < Size.Key(Size.Unit.BYTE); i++) _Key[i] = Convert.ToByte(key[(2 * i)..((2 * i) + 2)], 16);
        }
        catch
        {
            _Key = new Key()._Key;
        }
    }

    public override string ToString()
    {
        string acc = "";
        for (int i = 0; i < Size.Key(Size.Unit.BYTE); i++) acc += _Key[i].ToString("X2");
        return acc;
    }
}
