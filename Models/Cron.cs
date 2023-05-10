using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class Cron : BaseEntity
    {
        public long CronId { get; set; }
        public long OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
