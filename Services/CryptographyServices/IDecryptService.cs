using MaliehIran.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.CryptographyServices
{
    public interface IDecryptService
    {
        IServiceResult<string> Decrypt(string text);
        IServiceResult<string> DecryptMail(string text);
    }
}
