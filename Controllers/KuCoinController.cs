using MaliehIran.Services;
using MaliehIran.Services.KucoinServices;
using Kucoin.Net.Clients.SpotApi;
using Kucoin.Net.Enums;
using Kucoin.Net.Interfaces.Clients.FuturesApi;
using Kucoin.Net.Interfaces.Clients.SpotApi;
using Kucoin.Net.Objects.Models.Spot;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KuCoinController : ControllerBase
    {
        private readonly IKuCoinService kuCoinService;
        public KuCoinController(IKuCoinService kuCoinService)
        {
            this.kuCoinService = kuCoinService;
        }
        [HttpPost]
        [Route("ListAccounts")]
        public object ListAccounts(long cryptoAccountId)
        {
            try
            {
                var result =  kuCoinService.KucoinListAccounts(cryptoAccountId);
                return result;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        [Route("GetKlines")]
        public async Task<object> GetKlines(string symbol, string interval,long cryptoAccountId)
        {
            try
            {
                var asdf = (KlineInterval)Enum.Parse(typeof(KlineInterval), interval);
                var result =  kuCoinService.KucoinGetKlines(symbol,asdf, cryptoAccountId);
                return result;
            }
            catch(Exception ex)
            {
                return ex;
            }
        }

        [HttpGet]
        [Route("GetSymbolList")]
        public async Task<object> GetSymbolList(string symbol,long cryptoAccountId)
        {
            try
            {
                var result =  kuCoinService.KucoinGetSymbolList(symbol, cryptoAccountId);
                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        [HttpPost]
        [Route("PlaceOrder")]
        public string KucoinPlaceOrderLimit(long cryptoAccountId, string symbol, OrderSide side, decimal? quantity, decimal? price)
        {
            return kuCoinService.KucoinPlaceOrderLimit(cryptoAccountId, symbol,side,quantity,price);
        }

        [HttpPost]
        [Route("CancleOrder")]
        public KucoinCanceledOrders KucoinCancleOrder(long cryptoAccountId, string orderId)
        {
            return kuCoinService.KucoinCancleOrder(cryptoAccountId, orderId);
        }
        [HttpPost]
        [Route("GetOrder")]
        public KucoinOrder KucoinGetOrder(long cryptoAccountId, string orderId)
        {
            return kuCoinService.KucoinGetOrder(cryptoAccountId, orderId);
        }
        [HttpPost]
        [Route("GetOrders")]
        public object KucoinGetOrders(long cryptoAccountId)
        {
            return kuCoinService.KucoinGetOrders(cryptoAccountId);
        }

        [HttpPost]
        [Route("GetOrdersDone")]
        public object KucoinGetOrdersDone(long cryptoAccountId)
        {
            return kuCoinService.KucoinGetOrdersDone(cryptoAccountId);
        }

        [HttpPost]
        [Route("GetMarketInfo")]
        public object KucoinGetMarketInfo(string market,long cryptoAccountId)
        {
            return kuCoinService.KucoinGetMarketInfo(market, cryptoAccountId);
        }
    }
}
