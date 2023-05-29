using MaliehIran.Models;
using MaliehIran.Services.ShopServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;
        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }
        [HttpPost]
        public long Create(Shop model)
        {
            return _shopService.Create(model);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public long CreateByAdmin(Shop model)
        {
            return _shopService.CreateByAdmin(model);
        }
        [HttpPost]
        public async Task<Shop> Update(Shop model)
        {
            return await _shopService.Update(model);
        }
        [HttpPost]
        public Shop Get(long id)
        {
            return _shopService.Get(id);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public object GetAll(int pageNumber, int count, long? userId, string? searchCommand)
        {
            return _shopService.GetAll(pageNumber, count, userId, searchCommand);
        }
        [HttpPost]
        public object GetByUserId(int pageNumber, int count, string? searchCommand)
        {
            return _shopService.GetByUserId(pageNumber,count, searchCommand);
        }
        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await _shopService.Delete(id);
        }
    }
}
