using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class Candle
    {
        public string Time { get; set; }
        public string Open { get; set; }
        public string Close { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
    }
    public class CandleApiResult
    {
        public int code { get; set; }

        public List<List<object>> data { get; set; }
        public string message { get; set; }
    }
    public class OrderApiResult
    {
        public int code { get; set; }
        public OrderClass data { get; set; }
        public string message { get; set; }

    }
    public class OrderClass
    {
        public string amount { get; set; }
        public string avg_price { get; set; }
        public long? create_time { get; set; }
        public string deal_amount { get; set; }
        public string deal_money { get; set; }
        public long? finished_time { get; set; }
        public long id { get; set; }
        public string maker_fee_rate { get; set; }
        public string market { get; set; }
        public string order_type { get; set; }
        public string price { get; set; }
        public string status { get; set; }
        public string taker_fee_rate { get; set; }
        public string type { get; set; }
        public string client_id { get; set; }
        public string stock_fee { get; set; }
        public string money_fee { get; set; }
        public string asset_fee { get; set; }
        public string fee_asset { get; set; }
        public string fee_discount { get; set; }
        public string left { get; set; }
        public string source_id { get; set; }
    }
    public class AccountApiResult
    {
        public int code { get; set; }
        public Dictionary<string,AccountData> data { get; set; }
        public string message { get; set; }

    }
    public class AccountData
    {
        public string frozen { get; set; }
        public string available { get; set; }
    }
    public class MarketInfoResult
    {
        public int code { get; set; }
        public MarketData data { get; set; }
        public string message { get; set; }

    }
    public class MarketData
    {
        public string name { get; set; }
        public string min_amount { get; set; }
        public string maker_fee_rate { get; set; }
        public string taker_fee_rate { get; set; }
        public string pricing_name { get; set; }
        public decimal pricing_decimal { get; set; }
        public string trading_name { get; set; }
        public decimal trading_decimal { get; set; }
    }
}
