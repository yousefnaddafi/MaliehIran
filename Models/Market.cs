using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class Market : BaseEntity
    {
        public long MarketId { get; set; }
        public string MarketName { get; set; }
        public MarketType Type { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
