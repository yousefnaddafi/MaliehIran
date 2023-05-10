namespace MaliehIran.Models
{
    public class MessageRecipient : BaseEntity
    {
        public long MessageRecipientId { get; set; }
        public long? UserId { get; set; }
        public long? GroupId { get; set; }
        public long? ChannelId { get; set; }
        public long MessageId { get; set; }
        public bool IsRead { get; set; }
        public bool IsDelivered { get; set; }
    }
}
