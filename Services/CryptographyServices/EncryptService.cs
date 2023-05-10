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
    internal class EncryptService : IEncryptService
    {
        private readonly ICryptographySetting _setting;

        public EncryptService(ICryptographySetting setting)
        {
            _setting = setting;
        }
        public IServiceResult<string> Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new ServiceResult<string>().Failure("Empty");


            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_setting.Key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(text);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return new ServiceResult<string>().Ok(Convert.ToBase64String(array));

        }
    }
}
