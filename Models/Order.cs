using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class Order : BaseEntity
    {
        public long OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public ExchangeType Exchange { get; set; }
        public string Market { get; set; }
        public TradeTypes Type{ get; set; }
        public OrderStatus Status { get; set; }
        public decimal? Percent { get; set; }
        public string? Amount { get; set; }
        public decimal RiskAversion { get; set; }
        public int Length { get; set; }
        public string TimeFrame { get; set; }
        public int Method { get; set; }
        public long UserId { get; set; }
        public decimal MainOrderPrice { get; set; }
        public string BlockchainOrderId { get; set; }
        public long CryptoAccountId { get; set; }
    }
}
