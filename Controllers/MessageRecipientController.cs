using MaliehIran.Models;
using MaliehIran.Services.MessageRecipientServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageRecipientController : ControllerBase
    {
        private readonly IMessageRecipientService messageRecipientService;

        public MessageRecipientController(IMessageRecipientService messageRecipientService)
        {
            this.messageRecipientService = messageRecipientService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task Create([FromBody] MessageRecipient inputDto)
        {
            await messageRecipientService.Create(inputDto);
        }

        [HttpPost]
        [Route("[action]")]
        public List<MessageRecipient> GetByUserId(long userId)
        {
            return messageRecipientService.GetByUserId(userId);
        }

        [HttpPost]
        [Route("[action]")]
        public Task<MessageRecipient> Get(long logId)
        {
            return messageRecipientService.Get(logId);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task Delete(long id)
        {
            await messageRecipientService.Delete(id);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<List<MessageRecipient>> GetAll()
        {
            return await messageRecipientService.GetAll();
        }
    }
}
