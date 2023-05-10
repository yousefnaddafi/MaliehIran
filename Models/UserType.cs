using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Models
{
    public class UserType : BaseEntity
    {
        public long UserTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
