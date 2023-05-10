using MaliehIran.Models.Enums;
using MaliehIran.Models.Urls;
using MaliehIran.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using MaliehIran.Infrastructure;
using MaliehIran.Services.MediaServices;
using MaliehIran.Extensions;
using System.Linq;
using System.Text.Json;

namespace MaliehIran.Services.MessageServices
{
    public class MessageService : Hub<IMessageService>, IMessageService
    {
        private readonly IProjectEFRepository<Message> messageRepository;
        private readonly IProjectEFRepository<MessageRecipient> messageRecipientRepository;
        private readonly IProjectEFRepository<Media> mediaRepository;
        private readonly IHubContext<MessageService> hubContext;
        private readonly IMediaService _mediaService;
        private readonly IHttpContextAccessor _accessor;
        public MessageService(IProjectEFRepository<Message> messageRepository,  IMediaService mediaService, IProjectEFRepository<Media> mediaRepository, IHubContext<MessageService> hubContext, IHttpContextAccessor accessor, IProjectEFRepository<MessageRecipient> messageRecipientRepository)
        {
            this.messageRepository = messageRepository;
            _accessor = accessor;
            this.messageRecipientRepository = messageRecipientRepository;
            this.hubContext = hubContext;
            this.mediaRepository = mediaRepository;
            _mediaService = mediaService;
        }
        public async Task Create(Message inputDto)
        {
            messageRepository.Insert(inputDto);
        }

        public async Task<long> SendMessage(Message inputDto)
        {
            try
            {
                inputDto.UserId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
                inputDto.MessageId = 0;
                inputDto.IsDeleted = false;
                inputDto.CreateDate = DateTime.Now;
                var dbMessage = messageRepository.Insert(inputDto);
                if (inputDto.FileData != null)
                {
                    var fileName = string.Empty;
                    var filedata = inputDto.FileData;

                    if (filedata.Priority != 2)
                    {
                        fileName = _mediaService.SaveImage(filedata.Base64, EntityUrls.DMMediaUrl).ImageName;
                    }
                    else
                    {
                        fileName = string.Concat(Guid.NewGuid().ToString(), ".mp3");
                        var byteData = Convert.FromBase64String(filedata.Base64);
                        await File.WriteAllBytesAsync($"{Directory.GetCurrentDirectory()}/{EntityUrls.DMMediaUrl}/{fileName}", byteData);
                    }

                    mediaRepository.Insert(new Media
                    {
                        ObjectId = dbMessage.MessageId,
                        IsDeleted = false,
                        PictureUrl = fileName,
                        Type = MediaTypes.ChatMedia,
                        Priority = filedata.Priority,
                        MediaId = 0,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now
                    });
                }
                var MessageRec = new MessageRecipient()
                {
                    IsDeleted = false,
                    GroupId = (inputDto.RecipientGroupId == null || inputDto.RecipientUserId == 0) ? null : inputDto.RecipientGroupId,
                    UserId = (inputDto.RecipientUserId == null || inputDto.RecipientUserId == 0) ? null : inputDto.RecipientUserId,
                    ChannelId = (inputDto.RecipientChannelId == null || inputDto.RecipientChannelId == 0) ? null : inputDto.RecipientChannelId,
                    IsRead = false,
                    IsDelivered = false,
                    MessageId = dbMessage.MessageId,
                    MessageRecipientId = 0
                };
                var MSG = messageRecipientRepository.Insert(MessageRec);
                await hubContext.Clients.All.SendAsync("SendMessage", inputDto.UserId);
                return MSG.MessageId;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<bool> ReadMessage(long userId)
        {
            var myselfId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
            var beforeRead = messageRecipientRepository.GetQuery().Where(z => z.UserId == myselfId && z.IsRead == true).Count();
            var cmd = "update dbo.MessageRecipients set IsRead = 1 " +
                " where MessageId in " +
                " (select M.MessageId from dbo.Messages M join dbo.MessageRecipients MR on MR.MessageId = M.MessageId" +
                $" where MR.UserId = {myselfId} and M.UserId = {userId} )";
            try
            {
                var MR = await messageRepository.DapperSqlQuery(cmd);
                //hubContext.Clients.All.SendAsync("ReadMessage", userId);
                var afterRead = messageRecipientRepository.GetQuery().Where(z => z.UserId == myselfId && z.IsRead == true).Count();
                if (afterRead != beforeRead)
                {
                    await hubContext.Clients.All.SendAsync("ReadMessage", myselfId);
                }
                return true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task DeliverMessage()
        {
            try
            {
                var userId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
                var dbBeforDeliver = messageRecipientRepository.GetQuery().Where(z => z.UserId == userId && z.IsDelivered == true).Count();
                string cmd = $"update dbo.MessageRecipients set IsDelivered = 1 where UserId = {userId}";
                await messageRecipientRepository.DapperSqlQuery(cmd);
                //hubContext.Clients.All.SendAsync("DeliverMessage", userId);
                var dbAfterDeliver = messageRecipientRepository.GetQuery().Where(z => z.UserId == userId && z.IsDelivered == true).Count();
                if (dbAfterDeliver != dbBeforDeliver)
                {
                    await hubContext.Clients.All.SendAsync("DeliverMessage", userId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Message> Update(Message item)
        {
            await messageRepository.Update(item);
            return item;
        }

        public async Task<List<Message>> GetAll()
        {
            List<Message> messages = new List<Message>();
            messages = messageRepository.GetQuery().ToList();
            return messages;
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                var message = messageRepository.GetQuery().FirstOrDefault(z => z.MessageId == id);
                var messageRecipient = messageRecipientRepository.GetQuery().FirstOrDefault(z => z.MessageId == id);
                messageRepository.Erase(message);
                messageRecipientRepository.Erase(messageRecipient);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<Message> Get(long id)
        {
            var message = messageRepository.GetQuery().FirstOrDefault(z => z.MessageId == id);
            return message;
        }

        public async Task<List<MessageDTO>> GetMenu(string searchCommand)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
            if (searchCommand == null)
            {
                searchCommand = "";
            }

            string NEWCmd = "select * from (select u.UserId,(u.Name + ' ' + u.FamilyName) as UserName,m.Subject as Subject,m.createDate as CreateDate,' ' as UserPhoto, " +
                " (select count(*) from dbo.Messages m " +
                " join dbo.MessageRecipients mr on m.MessageId = mr.MessageId " +
                $" where (mr.UserId = {userId}) and(m.UserId = tbl1.UserId or mr.UserId = tbl1.UserId) and mr.IsRead = 0) as UnReadCount " +
                " from( " +
                " select UserId, MAX(MessageId) as MessageId from( " +
                " (select mr.UserId, MAX(m.MessageId) as MessageId from dbo.Messages m " +
                " join dbo.MessageRecipients mr on m.MessageId = mr.MessageId " +
                $" where m.UserId = {userId} " +
                " group by mr.UserId) " +
                " union " +
                " (select m.UserId, MAX(m.MessageId) as MessageId from dbo.Messages m " +
                " join dbo.MessageRecipients mr on m.MessageId = mr.MessageId " +
                $" where mr.UserId = {userId} " +
                " group by m.UserId)) as tbl " +
                " group by UserId) tbl1 " +
                " join dbo.Messages m on tbl1.MessageId = m.MessageId " +
                $" join[User].[Users] u on u.UserId = tbl1.UserId) as Result where UserName like N'%{searchCommand}%' " +
                " order By Result.CreateDate Desc ";

            var messages = await messageRepository.DapperSqlQuery(NEWCmd);
            var SerializeObject = JsonSerializer.Serialize<object>(messages);
            var serializedMessage = JsonSerializer.Deserialize<List<MessageDTO>>(SerializeObject);
            foreach (var item in serializedMessage)
            {
                var dbUserPhoto = mediaRepository.GetQuery().FirstOrDefault(z => z.Type == MediaTypes.Profile && z.ObjectId == item.UserId);
                if (dbUserPhoto != null)
                {
                    item.UserPhoto = dbUserPhoto.PictureUrl;
                }
            }

            return serializedMessage;
        }

        public async Task<List<MessageDTO>> GetByUserId()
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
            string cmd = "select * from (select distinct 0 as GroupId, M.IsForwarded, " +
                " (select Count(M.MessageId) from dbo.Messages M join dbo.MessageRecipients MR on M.MessageId=MR.MessageId where M.UserId =SenderU.UserId and MR.IsRead =0) as NotReadCount, " +
                $" CASE WHEN SenderU.UserId = {userId} THEN RecieverU.UserId ELSE SenderU.UserId END as UserId , " +
                $" CASE WHEN SenderU.UserId = {userId} THEN RecieverU.Name +' '+ RecieverU.FamilyName ELSE SenderU.Name +' '+ SenderU.FamilyName END as UserName, " +
                " M.Subject ,M.CreateDate ,MR.IsRead, " +
                $" CASE WHEN SenderU.UserId = {userId} THEN SMReciever.PictureUrl ELSE SMSender.PictureUrl END as UserPhoto " +
                " from dbo.MessageRecipients MR " +
                "  join dbo.Messages M on MR.MessageId = M.MessageId " +
                "  join[User].users SenderU on SenderU.UserId = M.UserId " +
                "  join[User].Users RecieverU on RecieverU.UserId = MR.UserId " +
                "  join dbo.Medias SMSender on SMSender.ObjectId = M.UserId " +
                "  join dbo.Medias SMReciever on SMReciever.ObjectId = MR.UserId " +
                "  where M.MessageId in (select max(M.MessageId) as messageId from dbo.MessageRecipients MR " +
                "  join dbo.Messages M on MR.MessageId = M.MessageId " +
                "  join [User].users U on U.UserId = M.UserId " +
                $" where MR.UserId = {userId} or M.UserId = {userId} " +
                "  group by U.UserId) and SMSender.Type = 2  and SMReciever.Type =2 " +
                "  union " +
                "  select distinct G.GroupId, M.IsForwarded, " +
                "  (select Count(M.MessageId) from dbo.Messages M join dbo.MessageRecipients MR on M.MessageId=MR.MessageId where MR.GroupId = G.GroupId and MR.IsRead =0) as NotReadCount, " +
                " U.UserId as UserId, U.Name + ' ' + U.FamilyName as UserName,M.Subject,M.CreateDate,MR.IsRead , SMSender.PictureUrl as UserPhoto from dbo.UserGroups UG " +
                "  join dbo.Groups G on G.GroupId = UG.GroupId " +
                "  join dbo.MessageRecipients MR on MR.GroupId=G.GroupId " +
                "  join dbo.Messages M on M.MessageId=MR.MessageId " +
                "  join [User].Users U on U.UserId=M.UserId " +
                "  join dbo.Medias SMSender on SMSender.ObjectId = M.UserId " +
                "  join dbo.Medias SMReciever on SMReciever.ObjectId = G.GroupId " +
                "  where M.MessageId in (select max(M.MessageId) as messageId from dbo.MessageRecipients MR " +
                "  join dbo.Messages M on MR.MessageId = M.MessageId " +
                "  join [User].users U on U.UserId = M.UserId " +
                $"   where M.UserId = {userId} " +
                "  group by U.UserId) and SMReciever.Type = 3  and SMSender.Type = 2) as Result " +
                "  order By Result.CreateDate Desc ";

            string NEWCmd = "select * from (select u.UserId,(u.Name + ' ' + u.FamilyName) as UserName,m.Subject as Subject,m.createDate as CreateDate,' ' as UserPhoto, " +
                " (select count(*) from dbo.Messages m " +
                " join dbo.MessageRecipients mr on m.MessageId = mr.MessageId " +
                $" where (mr.UserId = {userId}) and(m.UserId = tbl1.UserId or mr.UserId = tbl1.UserId) and mr.IsRead = 0) as UnReadCount " +
                " from( " +
                " select UserId, MAX(MessageId) as MessageId from( " +
                " (select mr.UserId, MAX(m.MessageId) as MessageId from dbo.Messages m " +
                " join dbo.MessageRecipients mr on m.MessageId = mr.MessageId " +
                $" where m.UserId = {userId} " +
                " group by mr.UserId) " +
                " union " +
                " (select m.UserId, MAX(m.MessageId) as MessageId from dbo.Messages m " +
                " join dbo.MessageRecipients mr on m.MessageId = mr.MessageId " +
                $" where mr.UserId = {userId} " +
                " group by m.UserId)) as tbl " +
                " group by UserId) tbl1 " +
                " join dbo.Messages m on tbl1.MessageId = m.MessageId " +
                " join[User].[Users] u on u.UserId = tbl1.UserId " +
                //" join dbo.Medias SM on SM.ObjectId = u.UserId where Sm.Type= 2" +
                ") as Result" +
                " order By Result.CreateDate Desc ";

            var messages = await messageRepository.DapperSqlQuery(NEWCmd);
            var SerializeObject = JsonSerializer.Serialize<object>(messages);
            var serializedMessage = JsonSerializer.Deserialize<List<MessageDTO>>(SerializeObject);
            foreach (var item in serializedMessage)
            {
                var dbUserPhoto = mediaRepository.GetQuery().FirstOrDefault(z => z.Type == MediaTypes.Profile && z.ObjectId == item.UserId);
                if (dbUserPhoto != null)
                {
                    item.UserPhoto = dbUserPhoto.PictureUrl;
                }
            }

            return serializedMessage;
        }

        public async Task<object> GetUserConversation(long userId, long messageId, int count)
        {
            var MyselfId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
            var completationCmd = messageId == 0 ? "" : $"and M.MessageId < {messageId}";
            string cmd = "select M.UserId as CreatorUserId, M.MessageId , M.MessageBody , M.Subject , M.IsForwarded , M.CreateDate , M.ParentMessageId , MR.IsRead ,MR.IsDelivered " +
                " from dbo.Messages M " +
                " join dbo.MessageRecipients MR on M.MessageId = MR.MessageId " +
                //" join dbo.Medias SM on SM.ObjectId = M.UserId "+
                $" where MR.userId in ({userId},{MyselfId}) and M.userId in ({userId},{MyselfId})" +
                //$" and SM.Type =2" +
                $" {completationCmd}" +
                $" order by M.CreateDate Desc OFFSET 0 ROWS FETCH NEXT {count} ROWS ONLY ";
            var messages = await messageRepository.DapperSqlQuery(cmd);
            var SerializeObject = JsonSerializer.Serialize<object>(messages);
            var result = JsonSerializer.Deserialize<List<ConversationDTO>>(SerializeObject);
            //var dbUserMedia = mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == MyselfId && z.Type == MediaTypes.Profile);
            //var dbChaterMedia = mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == userId && z.Type == MediaTypes.Profile);
            result = result.OrderByDescending(z => z.MessageId).ToList();

            foreach (var conversationDTO in result)
            {
                var dbChatMedia = mediaRepository.GetQuery().FirstOrDefault(z => z.ObjectId == conversationDTO.MessageId && z.Type == MediaTypes.ChatMedia);
                if (dbChatMedia != null)
                {
                    conversationDTO.ChatMedia = dbChatMedia.PictureUrl;
                }
                else
                {
                    conversationDTO.ChatMedia = null;
                }

            }
            return result;
        }
        public async Task<object> GetGroupConversation(long groupId, long messageId, int count)
        {
            var MyselfId = _accessor.HttpContext.User.Identity.IsAuthenticated
                    ? _accessor.HttpContext.User.Identity.GetUserId()
                    : 0;
            var completationCmd = messageId == 0 ? "" : $"and M.MessageId < {messageId}";
            string cmd = "select M.UserId as CreatorUserId, M.MessageId , M.MessageBody , M.Subject , M.IsForwarded , M.CreateDate , M.ParentMessageId , MR.IsRead , MR.IsDelivered " +
                " from dbo.Messages M " +
                " join dbo.MessageRecipients MR on M.MessageId = MR.MessageId " +
                " join dbo.Medias SM on SM.ObjectId = M.UserId " +
                $" where MR.GroupId = {groupId} and SM.Type = 2 {completationCmd}" +
                $" order by M.CreateDate Desc OFFSET 0 ROWS FETCH NEXT {count} ROWS ONLY ";
            var messages = await messageRepository.DapperSqlQuery(cmd);
            var SerializeObject = JsonSerializer.Serialize<object>(messages);
            return SerializeObject;
        }
    }
}
