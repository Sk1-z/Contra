using System.Security.Cryptography;

namespace Contra.Security;

public partial class Cryptor
{
    private interface IEncryptor
    {
        byte[] Encrypt(byte[] msg);
    }

    private class KeyEncryptor : IEncryptor
    {
        private readonly byte[] _key;

        public KeyEncryptor(byte[] key)
        {
            _key = key;
        }

        public byte[] Encrypt(byte[] msg)
        {
            byte[] cipher;
            byte[] iv;

            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(_key, iv))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var bw = new BinaryWriter(cs))
                    {
                        bw.Write(msg);
                    }

                    cipher = ms.ToArray();
                }
            }

            using (MemoryStream ms = new())
            {
                using (BinaryWriter bw = new(ms))
                {
                    bw.Write(iv);
                    bw.Write(cipher);
                    bw.Flush();
                }

                return ms.ToArray();
            }
        }
    }

    private class PasswordEncryptor : IEncryptor
    {
        private readonly byte[] _salt;

        private readonly KeyEncryptor _encryptor;

        public PasswordEncryptor(string password)
        {
            using (var rfc = new Rfc2898DeriveBytes(password, Size.Salt(Size.Unit.Byte), Size.Iterations, HashAlgorithmName.SHA256))
            {
                _salt = rfc.Salt;
                _encryptor = new(rfc.GetBytes(Size.Key(Size.Unit.Byte)));
            }
        }

        public byte[] Encrypt(byte[] msg)
        {
            using (MemoryStream ms = new())
            {
                using (BinaryWriter bw = new(ms))
                {
                    bw.Write(_salt);
                    bw.Write(_encryptor.Encrypt(msg));
                    bw.Flush();
                }

                return ms.ToArray();
            }
        }
    }

    private class PseudoEncryptor : IEncryptor
    {
        public byte[] Encrypt(byte[] msg)
        {
            return msg;
        }
    }
}
