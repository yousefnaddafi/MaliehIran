using Kucoin.Net.Enums;
using Kucoin.Net.Objects.Models.Spot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.KucoinServices
{
    public interface IKuCoinService
    {
        List<KucoinAccount> KucoinListAccounts(long cryptoAccountId);
        List<KucoinKline> KucoinGetKlines(string symbol, KlineInterval interval,long cryptoAccountId);
        KucoinSymbol KucoinGetSymbolList(string symbol,long cryptoAccountId);
        string KucoinPlaceOrderLimit(long cryptoAccountId, string symbol, OrderSide side, decimal? quantity, decimal? price, System.Threading.CancellationToken ct = default);
        KucoinCanceledOrders KucoinCancleOrder(long cryptoAccountId, string orderId, System.Threading.CancellationToken ct = default);
        KucoinOrder KucoinGetOrder(long cryptoAccountId, string orderId, System.Threading.CancellationToken ct = default);
        object KucoinGetOrders(long cryptoAccountId);
        object KucoinGetOrdersDone(long cryptoAccountId);
        object KucoinGetMarketInfo( string market, long cryptoAccountId);
    }
}
