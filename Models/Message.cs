using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MaliehIran.Models
{
    public class Message : BaseEntity
    {
        public long MessageId { get; set; }
        public string Subject { get; set; }
        public long UserId { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ParentMessageId { get; set; }
        public bool IsForwarded { get; set; }
        [NotMapped]
        public long? RecipientUserId { get; set; }
        [NotMapped]
        public long? RecipientGroupId { get; set; }
        [NotMapped]
        public long? RecipientChannelId { get; set; }
        [NotMapped]
        public FileData FileData { get; set; }
        [NotMapped]
        public string? MediaUrl { get; set; }
    }

    public class MessageDTO
    {
        public long UserId { get; set; }
        public string Subject { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string UserPhoto { get; set; }
        public long UnReadCount { get; set; }
    }

    public class ConversationDTO
    {
        public long? CreatorUserId { get; set; }
        public long? MessageId { get; set; }
        public string MessageBody { get; set; }
        public string Subject { get; set; }
        public bool? IsForwarded { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? ParentMessageId { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDelivered { get; set; }
        public string ChatMedia { get; set; }
    }
}
