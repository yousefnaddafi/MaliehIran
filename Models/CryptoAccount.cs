using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class CryptoAccount : BaseEntity
    {
        public long CryptoAccountId { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string PassPhrase { get; set; }
        public DateTime CreateDate { get; set; }
        public ExchangeType Exchange { get; set; }
        public long UserId { get; set; }

    }
}
