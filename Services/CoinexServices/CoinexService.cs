using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.WebSockets;
using System.Net.Http;
using System.Net.Http.Headers;
using MaliehIran.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MaliehIran.Extensions;
using MaliehIran.Infrastructure;

namespace MaliehIran.Services.CoinexServices

{
    public class CoinexService : ICoinexService
    {
        public string baseUrl;
        public string accessId;
        public string secretKey;
        public HttpClient client;
        private readonly IProjectEFRepository<User> userRepository;
        private readonly IProjectEFRepository<CryptoAccount> cryptoAccountRepository;
        private readonly IHttpContextAccessor _accessor;

        public CoinexService(IProjectEFRepository<User> userRepository, IHttpContextAccessor accessor, IProjectEFRepository<CryptoAccount> cryptoAccountRepository)
        {
            this.baseUrl = "https://api.coinex.com";
            this.accessId = "EE79B1C0B241475AB7EFACB0CB33EB1E";
            this.secretKey = "64D98139293E5D06F8C8CCD65FF7711C6371140097088644";
            this.client = new HttpClient();
            this.userRepository = userRepository;
            _accessor = accessor;
            this.cryptoAccountRepository = cryptoAccountRepository;
        }

        public string Sign(Dictionary<string, object> args,long cryptoAccountId)
        {
            // add access_id and tonce to args
            //var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
            //    _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbcryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == cryptoAccountId);
            if(dbcryptoAccount != null)
            {
                args["access_id"] = dbcryptoAccount.ApiKey; //this.accessId;
                args["tonce"] = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                // sort args by name and add secret_key to the last
                var last_args = new[] { new KeyValuePair<string, object>("secret_key", dbcryptoAccount.SecretKey) };
                var sortedArgs = args.OrderBy(p => p.Key).Concat(last_args);
                var body = string.Join("&", sortedArgs.Select(p => $"{p.Key}={p.Value}"));
                //calc md5
                var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(body));
                return BitConverter.ToString(md5).Replace("-", string.Empty).ToUpper();
            }
            else
            {
                return null;
            }
        }

        public JsonElement Post(string path, Dictionary<string, object> args, string signature = null)
        {
            var json = JsonSerializer.Serialize(args);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var req = new HttpRequestMessage(HttpMethod.Post, this.baseUrl + path);
            req.Content = content;
            if (signature != null)
            {
                req.Headers.Add("authorization", signature);
            }
            var res = this.client.SendAsync(req).Result;
            var result = res.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<JsonElement>(result);
        }


        public JsonElement Delete(string path, Dictionary<string, object> args, string signature = null)
        {
            var url = this.baseUrl + path;
            if (args != null)
            {
                var param = string.Join("&", args.Select(p => $"{p.Key}={p.Value}"));
                url = url + "?" + param;
            }
            string result;
            if (signature != null)
            {
                var req = new HttpRequestMessage(HttpMethod.Delete, url);
                req.Headers.Add("authorization", signature);
                var res = client.SendAsync(req).Result;
                result = res.Content.ReadAsStringAsync().Result;
            }
            else
            {
                result = this.client.GetStringAsync(url).Result;
            }
            return JsonSerializer.Deserialize<JsonElement>(result);
        }

        public JsonElement Get(string path, Dictionary<string, object> args = null, string signature = null)
        {
            var url = this.baseUrl + path;
            if (args != null)
            {
                var param = string.Join("&", args.Select(p => $"{p.Key}={p.Value}"));
                url = url + "?" + param;
            }
            string result;
            if (signature != null)
            {
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.Add("authorization", signature);
                var res = client.SendAsync(req).Result;
                result = res.Content.ReadAsStringAsync().Result;
            }
            else
            {
                result = this.client.GetStringAsync(url).Result;
            }
            return JsonSerializer.Deserialize<JsonElement>(result);
        }

        public JsonElement MarketList()
        {
            return this.Get("/v1/market/list");
        }



        public JsonElement MarketDepth(string market, string merge)
        {
            var args = new Dictionary<string, object>
            {
                ["market"] = market,
                ["merge"] = merge
            };
            return this.Get("/v1/market/depth", args);
        }

        public JsonElement GetOrderStatus(string market, long orderId, long cryptoAccountId)
        {
            var args = new Dictionary<string, object>
            {
                ["market"] = market,
                ["id"] = orderId,
            };
            var signature = this.Sign(args,cryptoAccountId);
            return this.Get("/v1/order/status", args, signature);
        }
        public JsonElement AccountInfo(long cryptoAccountId)
        {
            var args = new Dictionary<string, object>();
            var signature = this.Sign(args,cryptoAccountId);
            return this.Get("/v1/balance/info", args, signature);
        }

        public JsonElement PlaceLimitOrder(string market, string type, string amount, string price,long cryptoAccountId)
        {
            var args = new Dictionary<string, object>
            {
                ["market"] = market,
                ["type"] = type,
                ["amount"] = amount,
                ["price"] = price
            };
            var signature = this.Sign(args,cryptoAccountId);
            return this.Post("/v1/order/limit", args, signature);
        }
        public JsonElement AcquireSingleMarketInfo(string market)
        {
            var args = new Dictionary<string, object>
            {
                ["market"] = market
            };
            return Get("/v1/market/detail", args);
        }

        public JsonElement AcquireKLineData(string market, int limit, string type,long cryptoAccountId)
        {
            if (limit == 0)
            {
                limit = 100;
            }

            var args = new Dictionary<string, object>
            {
                ["market"] = market,
                ["limit"] = limit,
                ["type"] = type
            };
            var signature = this.Sign(args, cryptoAccountId);
            return Get("/v1/market/kline", args,signature);
        }

        public JsonElement CancleOrder(long? id, string market, string type,long cryptoAccountId)
        {
            var cryptoAccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId== cryptoAccountId);
            Dictionary<string, object> a;
            if (id == null)
            {
                var args = new Dictionary<string, object>
                {
                    ["access_id"] = cryptoAccount.ApiKey,
                    ["market"] = market,
                    ["tonce"] = 0,
                };
                a = args;
            }
            else
            {
                var args = new Dictionary<string, object>
                {
                    ["access_id"] = cryptoAccount.ApiKey,
                    ["id"] = id,
                    ["market"] = market,
                    ["tonce"] = 0,
                    ["type"] = type
                };
                a = args;
            }

            var signature = Sign(a,cryptoAccountId);
            return Delete("/v1/order/pending", a, signature);
        }

        //public JsonElement CancleOrder(string market, long order_id, int timestamp, int? windowtime)
        //{
        //    var args = new Dictionary<string, object>
        //    {
        //        ["market"] = market,
        //        ["order_id"] = order_id,
        //        ["timestamp"] = timestamp,
        //        ["windowtime"] = windowtime
        //    };
        //    var signature = Sign(args);
        //    return Post("/perpetual/v1/order/cancle", args, signature);
        //}

        //public void Trade(string market, string amount, TradeTypes type, string timeFrame, int kandel, Methods methods, double QPercent, double UPercent, int roundTo)
        // {
        //    decimal MainOrderPrice = 0;
        //    bool EndProcess = false;
        //    long orderId = 0;
        //    string MyFrozenMarket = "";
        //    string MyAvailableMarket = "";

        //    while (EndProcess == false)
        //    {
        //        var KandelsList = new List<Candle>();
        //        var Kans = AcquireKLineData(market, kandel + 1, timeFrame).ToString();
        //        var desersial = JsonSerializer.Deserialize<CandleApiResult>(Kans);
        //        foreach(var item in desersial.data)
        //        {
        //            var k = new Candle
        //            {
        //                 Time= item[0].ToString(),
        //                 Open=item[1].ToString(),
        //                 Close=item[2].ToString(),
        //                 High=item[3].ToString(),
        //                 Low=item[4].ToString(),
        //                 Data1=item[5].ToString(),
        //                 Data2=item[6].ToString()
        //            };
        //            KandelsList.Add(k);
        //        }
        //        var MethodResult = CalculateAverage(methods, KandelsList.Take(kandel).ToList());
        //        if (type == TradeTypes.buy)
        //        {
        //            MethodResult -= (int)UPercent * MethodResult / 100;
        //        }
        //        else
        //        {
        //            MethodResult += (int)QPercent * MethodResult / 100;
        //        }
        //        var optimizedOrderPrice = Math.Round(MethodResult, roundTo);

        //        if(optimizedOrderPrice != MainOrderPrice)
        //        {
        //            if (orderId !=0)
        //            {
        //                CancleOrder(orderId, market, Enum.GetName(typeof(TradeTypes), type));
        //            }
        //            var OrderSetResult = PlaceLimitOrder(market, Enum.GetName(typeof(TradeTypes), type) , amount, optimizedOrderPrice.ToString()).ToString();
        //            var ApiDeserialize = JsonSerializer.Deserialize<OrderApiResult>(OrderSetResult);
        //            orderId = ApiDeserialize.data.id;
        //            MainOrderPrice = optimizedOrderPrice;

        //            var Accountinfo = AccountInfo().ToString();
        //            var AccountDeserialized = JsonSerializer.Deserialize<AccountApiResult>(Accountinfo);
        //            MyFrozenMarket = AccountDeserialized.data.FirstOrDefault(z=> market.ToUpper().Contains(z.Key)).Value.frozen;
        //            MyAvailableMarket= AccountDeserialized.data.FirstOrDefault(z => market.ToUpper().Contains(z.Key)).Value.available;
        //        }
        //        else
        //        {
        //            var Accountinfo = AccountInfo().ToString();
        //            var AccountDeserialized = JsonSerializer.Deserialize<AccountApiResult>(Accountinfo);
        //            var frozen = AccountDeserialized.data.FirstOrDefault(z => market.ToUpper().Contains(z.Key)).Value.frozen;
        //            var available = AccountDeserialized.data.FirstOrDefault(z => market.ToUpper().Contains(z.Key)).Value.available;
        //            if(frozen != MyFrozenMarket && available != MyAvailableMarket)
        //            {
        //                EndProcess = true;
        //            }
        //        }

        //    }
        //}
        public decimal CalculateAverage(Methods method,List<Candle> kandels)
        {
            decimal result=0;
            switch ((int)method)
            {
                case (1):
                    result= kandels.Select(z => Convert.ToDecimal(z.Open)).Average();
                    break;
                case (2):
                    result = kandels.Select(z => Convert.ToDecimal(z.Close)).Average();
                    break;
                case (3):
                    result = kandels.Select(z => Convert.ToDecimal(z.High)).Average();
                    break;
                case (4):
                    result = kandels.Select(z => Convert.ToDecimal(z.Low)).Average();
                    break;
                case (5):
                    result = kandels.Select(z => (Convert.ToDecimal(z.High) + Convert.ToDecimal(z.Low)) / 2).Average();
                    break;
                case (6):
                    result = kandels.Select(z => (Convert.ToDecimal(z.Open) + Convert.ToDecimal(z.Close)) / 2).Average();
                    break;
                case (7):
                    result = kandels.Select(z => (Convert.ToDecimal(z.Open) + Convert.ToDecimal(z.Close) + Convert.ToDecimal(z.High) + Convert.ToDecimal(z.Low)) / 4).Average();
                    break;
            }
            return result;
        }

    }
}
