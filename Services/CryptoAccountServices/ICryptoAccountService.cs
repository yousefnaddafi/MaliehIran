using MaliehIran.Models;
using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.CryptoAccountServices
{
    public interface ICryptoAccountService
    {
        long Create(CryptoAccount model);
        Task<CryptoAccount> Update(CryptoAccount model);
        Task<bool> Delete(long id);
        CryptoAccount Get(long id);
        object GetAll(int pageNumber, int count, ExchangeType? exchange);
        object GetByUserId(int pageNumber, int count, ExchangeType? exchange);
    }
}
