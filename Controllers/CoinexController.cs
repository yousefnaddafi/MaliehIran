using MaliehIran.Models;
using MaliehIran.Services;
using MaliehIran.Services.CoinexServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinexController : ControllerBase
    {
        private readonly ICoinexService cryptoService;
        public CoinexController(ICoinexService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

        [HttpGet]
        [Route("[action]")]
        public JsonElement MarketList()
        {
            var result = cryptoService.MarketList();
            return result;
        }

        [HttpGet]
        [Route("[action]")]
        public JsonElement AccountInfo(long cryptoAccountId)
        {
            var result = cryptoService.AccountInfo(cryptoAccountId);
            return result;
        }

        [HttpPost]
        [Route("[action]")]
        public JsonElement PlaceLimitOrder(string market, string type, string amount, string price,long cryptoAccountId)
        {
            var result = cryptoService.PlaceLimitOrder(market, type, amount, price, cryptoAccountId);
            return result;
        }

        [HttpPost]
        [Route("[action]")]
        public JsonElement AcquireSingleMarketInfo(string market)
        {
            var result = cryptoService.AcquireSingleMarketInfo(market);
            return result;
        }

        [HttpPost]
        [Route("[action]")]
        public JsonElement AcquireKLineData(string market, int limit, string type,long cryptoAccountId)
        {
            var result = cryptoService.AcquireKLineData(market,limit,type, cryptoAccountId);
            return result;
        }

        [HttpDelete]
        [Route("[action]")]
        public JsonElement CancleOrder(long? id, string market, string type, long cryptoAccountId)
        {

            var result = cryptoService.CancleOrder( id, market,type, cryptoAccountId);
            return result;
        }

        //[HttpPost]
        //[Route("[action]")]
        //public void Trade(string market, string amount,TradeTypes type,string timeFrame,int kandel,Methods methods,double QPercent,double UPercent,int roundTo,long userId)
        //{
        //    cryptoService.Trade(market, amount, type, timeFrame, kandel, methods, QPercent, UPercent, roundTo, userId);
        //}
    }
}
