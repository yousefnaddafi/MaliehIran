using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.CryptoAccountServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CryptoAccountController : ControllerBase
    {
        private readonly ICryptoAccountService cryptoAccountService;
        public CryptoAccountController(ICryptoAccountService cryptoAccountService)
        {
            this.cryptoAccountService = cryptoAccountService;
        }

        [HttpPost]
        public long Create(CryptoAccount model)
        {
            return cryptoAccountService.Create(model);
        }

        [HttpPost]
        public async Task<CryptoAccount> Update(CryptoAccount model)
        {
            return await cryptoAccountService.Update(model);
        }

        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await cryptoAccountService.Delete(id);
        }

        [HttpPost]
        public CryptoAccount Get(long id)
        {
            return cryptoAccountService.Get(id);
        }

        [HttpPost]
        public object GetAll(int pageNumber, int count, ExchangeType? exchange)
        {
            return cryptoAccountService.GetAll(pageNumber,count,exchange);
        }
        [HttpPost]
        public object GetByUserId(int pageNumber, int count, ExchangeType? exchange)
        {
            return cryptoAccountService.GetByUserId(pageNumber, count, exchange);
        }
        
    }
}
