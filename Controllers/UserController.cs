using MaliehIran.Models;
using MaliehIran.Services.Common;
using MaliehIran.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost]
        public async Task<long> Create(User inputDto)
        {
            return await userService.Create(inputDto);
        }

        [HttpPost]
        public async Task<IServiceResult<User>> Update(User item)
        {
            return await userService.Update(item);
        }

        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await userService.Delete(id);
        }

        [HttpPost]
        public async Task<object> GetAll(int pageNumber, int count, string searchCommand)
        {
            return await userService.GetAll(pageNumber,count,searchCommand);
        }

        [HttpPost]
        public async Task<object> ChangeUserRole(long userId, long userRole)
        {
            return await userService.ChangeUserRole(userId, userRole);
        }
        [HttpPost]
        public User Get(long id)
        {
            return userService.Get(id);
        }

        [HttpPost]
        public User GetById()
        {
            return userService.GetById();
        }
        

    }
}
