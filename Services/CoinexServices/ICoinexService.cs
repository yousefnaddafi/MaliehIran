using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaliehIran.Services.CoinexServices
{
    public interface ICoinexService
    {
        string Sign(Dictionary<string, object> args,long cryptoAccountId);
        JsonElement Post(string path, Dictionary<string, object> args, string signature = null);
        JsonElement Get(string path, Dictionary<string, object> args = null, string signature = null);
        JsonElement MarketList();
        JsonElement MarketDepth(string market, string merge);
        JsonElement AccountInfo(long cryptoAccountId);
        JsonElement PlaceLimitOrder(string market, string type, string amount, string price, long cryptoAccountId);
        JsonElement AcquireSingleMarketInfo(string market);
        JsonElement AcquireKLineData(string market, int limit, string type,long cryptoAccountId);
        JsonElement CancleOrder(long? id, string market, string type, long cryptoAccountId);
        //JsonElement CancleOrder(string market, long order_id, int timestamp, int? windowtime);
        //void Trade(string market, string amount, TradeTypes type, string timeFrame, int kandel, Methods methods, double QPercent, double UPercent, int roundTo);
        JsonElement GetOrderStatus(string market, long orderId, long cryptoAccountId);
    }
}
