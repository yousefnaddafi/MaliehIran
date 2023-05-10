using MaliehIran.Models;
using MaliehIran.Services.UserGroupServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageUserGroupController : ControllerBase
    {
        private readonly IUserGroupService userGroupService;

        public MessageUserGroupController(IUserGroupService userGroupService)
        {
            this.userGroupService = userGroupService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task Create([FromBody] UserGroup inputDto)
        {
            await this.userGroupService.Create(inputDto);
        }


        [HttpPost]
        [Route("[action]")]
        public Task<UserGroup> Get(long logId)
        {
            return userGroupService.Get(logId);
        }
        [HttpPost]
        [Route("[action]")]
        public List<UserGroup> GetByUserId(long userId)
        {
            return userGroupService.GetByUserId(userId);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task Delete(long id)
        {
            await userGroupService.Delete(id);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<List<UserGroup>> GetAll()
        {
            return await userGroupService.GetAll();
        }
    }
}
