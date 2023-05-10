using MaliehIran.Models;
using MaliehIran.Services.MarketServices;
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
    public class MarketController : ControllerBase
    {
        private readonly IMarketService marketService;
        public MarketController(IMarketService marketService)
        {
            this.marketService = marketService;
        }
        [HttpPost]
        public long Create(Market model)
        {
            return marketService.Create(model);
        }

        [HttpPost]
        public async Task<Market> Update(Market model)
        {
            return await marketService.Update(model);
        }

        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await marketService.Delete(id);
        }

        [HttpPost]
        public Market Get(long id)
        {
            return marketService.Get(id);
        }

        [HttpPost]
        public object GetAll(int pageNumber, int count, string searchCommand)
        {
            return marketService.GetAll(pageNumber,count,searchCommand);
        }
        
    }
}
