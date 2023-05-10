
using System;
using System.Collections.Generic;
using System.Net.Http;

using Kucoin.Net.Clients.SpotApi;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Buffers.Text;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Kucoin.Net.Clients;
using Kucoin.Net.Objects;
using Kucoin.Net.Objects.Models.Spot;
using Kucoin.Net.Enums;
using System.Linq;
using System.Threading;
using MaliehIran.Infrastructure;
using MaliehIran.Models;
using Microsoft.AspNetCore.Http;
using MaliehIran.Extensions;

namespace MaliehIran.Services.KucoinServices
{
    public class KuCoinService : IKuCoinService 
    {
        public string baseUrl;
        public string ApiKeyKucoin;
        public string secretKeyKucoin;
        public string passPhraseKucoin;
        public HttpClient client;
        private readonly IProjectEFRepository<User> userRepository;
        private readonly IProjectEFRepository<CryptoAccount> cryptoAccountRepository;
        private readonly IHttpContextAccessor _accessor;
        public KuCoinService(IProjectEFRepository<User> userRepository,IHttpContextAccessor accessor, IProjectEFRepository<CryptoAccount> cryptoAccountRepository)
        {
            this.baseUrl = "https://api.kucoin.com";
            this.ApiKeyKucoin = "62c01e673e5f0500014db114";
            this.secretKeyKucoin = "458e9c5d-8d56-48ee-87a7-0f9a1932d5dc";
            this.passPhraseKucoin = "KuK..1373";
            this.userRepository = userRepository;
            _accessor = accessor;
            this.cryptoAccountRepository = cryptoAccountRepository;
        }

        public List<KucoinAccount> KucoinListAccounts(long cryptoAccountId)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if(dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.Account.GetAccountsAsync().Result;
                var finalResult = result.Data.ToList();
                return finalResult;
            }
            else
            {
                return null;
            }

        }

        public List<KucoinKline> KucoinGetKlines(string symbol, KlineInterval interval, long cryptoAccountId)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.ExchangeData.GetKlinesAsync(symbol, interval).Result;
                var finalResult = result.Data.ToList();
                return finalResult;
            }
            else
            {
                return null;
            }
        }

        public KucoinSymbol KucoinGetSymbolList(string symbol,long cryptoAccountId)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.ExchangeData.GetSymbolsAsync().Result;
                var finalResult = result.Data.Where(z => z.Symbol.Contains(symbol.ToUpper() + "-USDT")).FirstOrDefault();
                return finalResult;
            }
            else
            {
                return null;
            }
        }

        //public List<KucoinSymbol> KucoinGetSymbolList(string symbol)
        //{
        //    KucoinClient client = new KucoinClient();
        //    var credentials = new KucoinApiCredentials(apiKey: ApiKeyKucoin, apiSecret: secretKeyKucoin, apiPassPhrase: passPhraseKucoin);
        //    client.SetApiCredentials(credentials);
        //    var result = client.SpotApi.ExchangeData.GetSymbolsAsync().Result;
        //    var finalResult = result.Data.Where(z=>z.Symbol.Contains(symbol.ToUpper() + "-USDT")).ToList();
        //    return finalResult;
        //}

        public string KucoinPlaceOrderLimit(long cryptoAccountId, string symbol, OrderSide side, decimal? quantity, decimal? price ,System.Threading.CancellationToken ct = default)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.Trading.PlaceOrderAsync(
                    symbol, side, NewOrderType.Limit, quantity, price, null, TimeInForce.GoodTillCanceled, null,
                    null, true, null, null, null, null, null, ct).Result;
                var finalResult = result.Data.Id;
                return finalResult;
            }
            else
            {
                return null;
            }
        }

        public KucoinCanceledOrders KucoinCancleOrder(long cryptoAccountId, string orderId, CancellationToken ct = default)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.Trading.CancelOrderAsync(orderId, ct).Result;
                var finalResult = result.Data;
                return finalResult;
            }
            else
            {
                return null;
            }
        }
        public KucoinOrder KucoinGetOrder(long cryptoAccountId, string orderId, CancellationToken ct = default)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.Trading.GetOrderAsync(orderId, ct).Result;
                var finalResult = result.Data;
                return finalResult;
            }
            else
            {
                return null;
            }
        }
        public object KucoinGetOrders(long cryptoAccountId)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.Trading.GetOrdersAsync(null, null, null, null, null, OrderStatus.Active).Result;
                var finalResult = result.Data;
                return finalResult;
            }
            else
            {
                return null;
            }
        }
        public object KucoinGetOrdersDone(long cryptoAccountId)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.SpotApi.Trading.GetOrdersAsync(null, null, null, null, null, OrderStatus.Done).Result;
                var finalResult = result.Data;
                return finalResult;
            }
            else
            {
                return null;
            }
        }

        public object KucoinGetMarketInfo(string market,long cryptoAccountId)
        {
            KucoinClient client = new KucoinClient();
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbCryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if (dbCryptoAccount != null)
            {
                var credentials = new KucoinApiCredentials(apiKey: dbCryptoAccount.ApiKey, apiSecret: dbCryptoAccount.SecretKey, apiPassPhrase: dbCryptoAccount.PassPhrase);
                client.SetApiCredentials(credentials);
                var result = client.FuturesApi.ExchangeData.GetCurrentMarkPriceAsync(market).Result;
                var finalResult = result.Data;
                return finalResult;
            }
            else
            {
                return false;
            }
        }
    }
}
