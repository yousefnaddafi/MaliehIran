using System;

namespace MaliehIran.Models
{
    public class Shop : BaseEntity
    {
        public long ShopId { get; set; }
        public long UserId { get; set; }
        public string? NationalId { get; set; }
        public string? Address { get; set; }
        public string? TaxUnit { get; set; }
        public string? FileNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public string ShopName { get; set; }
        public string? TerminalNumber { get; set; }
        public DateTime CompanyStartDate { get; set; }
    }
}
