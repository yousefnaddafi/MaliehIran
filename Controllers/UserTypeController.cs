using MaliehIran.Models;
using MaliehIran.Services.UserTypeServices;
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
    public class UserTypeController : ControllerBase
    {
        private readonly IUserTypeService userTypeService;
        public UserTypeController(IUserTypeService userTypeService)
        {
            this.userTypeService = userTypeService;
        }
        [HttpPost]
        public async Task<long> Create(UserType inputDto)
        {
            return await userTypeService.Create(inputDto);
        }

        [HttpPost]
        public async Task<UserType> Update(UserType item)
        {
            return await userTypeService.Update(item);
        }

        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await userTypeService.Delete(id);
        }

        [HttpPost]
        public async Task<List<UserType>> GetAll()
        {
            return await userTypeService.GetAll();
        }

        [HttpPost]
        public async Task<UserType> Get(long id)
        {
            return await userTypeService.Get(id);
        }

        [HttpPost]
        public List<UserType> GetByCommand(string sql)
        {
            return userTypeService.GetByCommand(sql);
        }

        [HttpPost]
        public long UpdateByCmd(string name, long id)
        {
            return userTypeService.UpdateByCmd(name,id);
        }
    }
        
}
