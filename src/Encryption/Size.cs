namespace Contra.Security;

public static class Size
{
    public const int ITERATIONS = 4096;

    private const int KEYBITSIZE = 256;
    private const int BLOCKBITSIZE = 128;
    private const int SALTBITSIZE = 64;

    public enum Unit
    {
        BIT = 1,
        NIBBLE = 4,
        BYTE = 8,
    }

    public static int Key(Unit unit)
    {
        return KEYBITSIZE / (int)unit;
    }

    public static int Block(Unit unit)
    {
        return BLOCKBITSIZE / (int)unit;
    }

    public static int Salt(Unit unit)
    {
        return SALTBITSIZE / (int)unit;
    }
}
