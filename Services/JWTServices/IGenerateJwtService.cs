using MaliehIran.Models;
using MaliehIran.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.JWTServices
{
    public interface IGenerateJwtService
    {
        Task<IServiceResult<object>> GenerateAsync(User user);
    }
}
