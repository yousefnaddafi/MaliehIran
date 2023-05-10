using MaliehIran.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.CryptographyServices
{
    public interface IEncryptService
    {
        IServiceResult<string> Encrypt(string text);
    }
}
