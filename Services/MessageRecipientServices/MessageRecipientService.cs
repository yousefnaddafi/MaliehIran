using MaliehIran.Infrastructure;
using MaliehIran.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.MessageRecipientServices
{
    public class MessageRecipientService : IMessageRecipientService
    {
        private readonly IProjectEFRepository<MessageRecipient> messageRecipientRepository;
        public MessageRecipientService(IProjectEFRepository<MessageRecipient> messageRecipientRepository)
        {
            this.messageRecipientRepository = messageRecipientRepository;
        }
        public async Task Create(MessageRecipient inputDto)
        {
            messageRecipientRepository.Insert(inputDto);
        }

        public async Task<MessageRecipient> Update(MessageRecipient item)
        {
            await messageRecipientRepository.Update(item);
            return item;
        }

        public async Task<List<MessageRecipient>> GetAll()
        {
            List<MessageRecipient> messageRecipients = new List<MessageRecipient>();
            messageRecipients = messageRecipientRepository.GetQuery().ToList();
            return messageRecipients;
        }

        public async Task Delete(long id)
        {
            var messageRecipient = messageRecipientRepository.GetQuery().FirstOrDefault(z => z.MessageRecipientId == id);
            await messageRecipientRepository.Delete(messageRecipient);
        }

        public async Task<MessageRecipient> Get(long id)
        {
            var messageRecipient = messageRecipientRepository.GetQuery().FirstOrDefault(z => z.MessageRecipientId == id);
            return messageRecipient;
        }
        public List<MessageRecipient> GetByUserId(long userId)
        {
            return messageRecipientRepository.GetQuery().Where(z => z.UserId == userId).ToList();
        }
    }
}
