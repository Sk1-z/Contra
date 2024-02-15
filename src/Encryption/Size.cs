namespace Contra.Security;

public static class Size
{
    public const int Iterations = 4096;

    private const int KeySize = 256;
    private const int BlockSize = 128;
    private const int SaltSize = 64;

    public enum Unit
    {
        Bit = 1,
        Nibble = 4,
        Byte = 8,
    }

    public static int Key(Unit unit)
    {
        return KeySize / (int)unit;
    }

    public static int Block(Unit unit)
    {
        return BlockSize / (int)unit;
    }

    public static int Salt(Unit unit)
    {
        return SaltSize / (int)unit;
    }
}
