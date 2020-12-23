using System;
using System.Security.Cryptography;
using System.Text;

namespace SecretSantaApplication.Services
{
    public class PasswordSecurity
    {
        private const string Hash = "Password_Hash";

        public string Encrypt(string password)
        {
            var data = Encoding.UTF8.GetBytes(password);
            using MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var keys = md5.ComputeHash(Encoding.UTF8.GetBytes(Hash));
            using TripleDESCryptoServiceProvider tripleDesCryptoServiceProvider =
                new TripleDESCryptoServiceProvider()
                {
                    Key = keys,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
            ICryptoTransform cryptoTransform = tripleDesCryptoServiceProvider.CreateEncryptor();
            var result = cryptoTransform.TransformFinalBlock(data, 0, data.Length);
            var encryptedPassword = Convert.ToBase64String(result, 0, result.Length);
            return encryptedPassword;
        }
    }
}