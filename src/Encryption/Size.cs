namespace Contra.Encryption;

public static partial class Size
{
    private const int _ITERATIONS = 4096;

    private const int _KEYBITSIZE = 256;
    private const int _BLOCKBITSIZE = 128;
    private const int _SALTBITSIZE = 64;

    public enum Unit
    {
        BIT = 1,
        NIBBLE = 4,
        BYTE = 8,
    }

    public static int Key(Unit unit)
    {
        return _KEYBITSIZE / (int)unit;
    }

    public static int Block(Unit unit)
    {
        return _BLOCKBITSIZE / (int)unit;
    }

    public static int Salt(Unit unit)
    {
        return _SALTBITSIZE / (int)unit;
    }
}
