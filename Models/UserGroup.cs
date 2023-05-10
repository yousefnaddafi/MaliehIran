using System;

namespace MaliehIran.Models
{
    public class UserGroup : BaseEntity
    {
        public long UserGroupId { get; set; }
        public long UserId { get; set; }
        public long GroupId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
