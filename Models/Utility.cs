using MaliehIran.Models.Enums;
using System;

namespace MaliehIran.Models
{
    public class Utility : BaseEntity
    {
        public long UtilityId { get; set; }
        public long UserId { get; set; }
        public long ShopId { get; set; }
        public DateTime CreateDate { get; set; }
        public UtilityType Type { get; set; }
        public UtilityStatus Status { get; set; }
        public string? Description { get; set; }
    }
}
