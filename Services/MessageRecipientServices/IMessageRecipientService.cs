using MaliehIran.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaliehIran.Services.MessageRecipientServices
{
    public interface IMessageRecipientService
    {
        Task Create(MessageRecipient inputDto);
        Task<MessageRecipient> Update(MessageRecipient item);
        Task Delete(long id);
        Task<List<MessageRecipient>> GetAll();
        Task<MessageRecipient> Get(long id);
        List<MessageRecipient> GetByUserId(long userId);
    }
}
