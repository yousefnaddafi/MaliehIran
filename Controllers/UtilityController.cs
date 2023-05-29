using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.UtilityServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        private readonly IUtilityService utilityService;
        public UtilityController(IUtilityService utilityService)
        {
            this.utilityService = utilityService;
        }
        [HttpPost]
        public long Create(Utility model)
        {
            return utilityService.Create(model);
        }
        [HttpPost]
        public async Task<Utility> Update(Utility model)
        {
            return await utilityService.Update(model);
        }
        [HttpPost]
        public Utility Get(long id)
        {
            return utilityService.Get(id);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public object GetAll(int pageNumber, int count, long? userId, long? shopId, UtilityType? type, UtilityStatus? status)
        {
            return utilityService.GetAll(pageNumber, count, userId, shopId, type, status);
        }
        [HttpPost]
        public object GetByUserId(int pageNumber, int count, long? shopId, UtilityType? type, UtilityStatus? status)
        {
            return utilityService.GetByUserId(pageNumber, count, shopId, type, status);
        }
        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await utilityService.Delete(id);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<bool> ChangeStatus(long utilityId, UtilityStatus status)
        {
            return await utilityService.ChangeStatus(utilityId, status);
        }
    }
}
