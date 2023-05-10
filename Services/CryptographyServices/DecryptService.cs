using MaliehIran.Services.Common;
using MaliehIran.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MaliehIran.Services.CryptographyServices
{
    internal class DecryptService : IDecryptService
    {
        private readonly ICryptographySetting _setting;

        public DecryptService(ICryptographySetting setting)
        {
            _setting = setting;
        }
        public IServiceResult<string> Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new ServiceResult<string>().Failure("Empty");

            byte[] iv = new byte[16];
            text = text.Replace(" ", "+");
            byte[] buffer = Convert.FromBase64String(text);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_setting.Key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return new ServiceResult<string>().Ok(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }
        public IServiceResult<string> DecryptMail(string text)
        {
            var utcDate = DateTime.UtcNow;
            var keybytes = Encoding.UTF8.GetBytes("82f2ceed4c503896c8a291e560bd4325");
            var iv = Encoding.UTF8.GetBytes("YousefNaddafi");

            //DECRYPT FROM CRIPTOJS
            var encrypted = Convert.FromBase64String(text);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return new ServiceResult<string>().Ok(decriptedFromJavascript);
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
