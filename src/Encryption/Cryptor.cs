using System.Text;

namespace Contra.Security;

public partial class Cryptor
{
    private readonly IEncryptor _encryptor;
    private readonly IDecryptor _decryptor;

    public Cryptor()
    {
        _encryptor = new PseudoEncryptor();
        _decryptor = new PseudoDecryptor();
    }

    public Cryptor(Key key)
    {
        _encryptor = new KeyEncryptor(key.Bytes());
        _decryptor = new KeyDecryptor(key.Bytes());
    }

    public Cryptor(string password)
    {
        _encryptor = new PasswordEncryptor(password);
        _decryptor = new PasswordDecryptor(password);
    }

    public string Encrypt(string msg)
    {
        return Convert.ToBase64String(_encryptor.Encrypt(Encoding.UTF8.GetBytes(msg)));
    }

    public string Decrypt(string msg)
    {
        return Encoding.UTF8.GetString(_decryptor.Decrypt(Convert.FromBase64String(msg)));
    }
}
