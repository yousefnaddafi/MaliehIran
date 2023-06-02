using MaliehIran.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaliehIran.Models
{
    public class Report : BaseEntity
    {
        public long ReportId { get; set; }
        public string Title { get; set; }
        public long UserId { get; set; }
        public long ShopId { get; set; }
        public string? Description { get; set; }
        public ReportType Type { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public MediaInfo MediaInfo { get; set; }
        [NotMapped]
        public string ShopName { get; set; }
    }
}
