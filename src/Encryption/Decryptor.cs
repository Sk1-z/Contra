using System.Security.Cryptography;

namespace Contra.Security;

public partial class Cryptor
{
    private interface IDecryptor
    {
        byte[]? Decrypt(byte[] msg);
    }

    private class KeyDecryptor : IDecryptor
    {
        private byte[] _key;

        public KeyDecryptor(byte[] key)
        {
            _key = key;
        }

        public byte[]? Decrypt(byte[] encryptedMsg)
        {
            using (var aes = Aes.Create())
            {
                byte[] iv = new byte[128 / 8];
                Array.Copy(encryptedMsg, iv, iv.Length);

                using (var decryptor = aes.CreateDecryptor(_key, iv))
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        using (var bw = new BinaryWriter(cs))
                        {
                            bw.Write(
                                encryptedMsg,
                                iv.Length,
                                encryptedMsg.Length - iv.Length
                            );
                        }

                        return ms.ToArray();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
    }

    private class PasswordDecryptor : IDecryptor
    {
        private string _password;

        public PasswordDecryptor(string password)
        {
            _password = password;
        }

        public byte[]? Decrypt(byte[] msg)
        {
            byte[] salt = new byte[Size.Salt(Size.Unit.Byte)];
            Array.Copy(msg, salt, salt.Length);

            byte[] key = new Rfc2898DeriveBytes(
                    _password,
                    salt,
                    Size.Iterations,
                    HashAlgorithmName.SHA256
            ).GetBytes(Size.Key(Size.Unit.Byte));

            byte[] cipher = new byte[msg.Length - salt.Length];
            Array.Copy(msg, salt.Length, cipher, 0, cipher.Length);

            return new KeyDecryptor(key).Decrypt(cipher);
        }
    }


    private class PseudoDecryptor : IDecryptor
    {
        public byte[]? Decrypt(byte[] msg)
        {
            return msg;
        }
    }
}
