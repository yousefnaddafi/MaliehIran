using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using Org.BouncyCastle.Crypto.Prng.Drbg;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MaliehIran.Extensions;

namespace MaliehIran.Services.ShopServices
{
    public class ShopService : IShopService
    {
        private readonly IProjectEFRepository<Shop> _shopRepository;
        private readonly IHttpContextAccessor _accessor;
        public ShopService(IProjectEFRepository<Shop> shopRepository,IHttpContextAccessor accessor)
        {
            _shopRepository = shopRepository;
            _accessor = accessor;
        }
        public long Create(Shop model)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            model.UserId = userId;
            model.CreateDate = DateTime.Now;
            var creation = _shopRepository.Insert(model);
            return creation.ShopId;
        }
        public long CreateByAdmin(Shop model)
        {
            model.CreateDate= DateTime.Now;
            var creation = _shopRepository.Insert(model);
            return creation.ShopId;
        }
        public async Task<Shop> Update(Shop model)
        {
            return await _shopRepository.Update(model);
        }
        public Shop Get(long id)
        {
            var dbShop = _shopRepository.GetQuery().FirstOrDefault(z => z.ShopId == id);
            return dbShop;
        }
        public object GetAll(int pageNumber, int count, long? userId, string? searchCommand)
        {
            searchCommand = searchCommand ?? "";
            var dbShops = _shopRepository.GetQuery().Where(z => z.ShopName.Contains(searchCommand) || z.TaxUnit.Contains(searchCommand)
                || z.TerminalNumber.Contains(searchCommand) || z.Address.Contains(searchCommand)|| z.FileNumber.Contains(searchCommand)
                || z.NationalId.Contains(searchCommand)).ToList();

            if (userId != null)
            {
                dbShops = dbShops.Where(z => z.UserId == userId).ToList();
            }

            var dbCount = dbShops.Count();

            if (pageNumber != 0 && count != 0)
            {
                dbShops = dbShops.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            var result = new
            {
                Shops = dbShops,
                Count = dbCount
            };
            return result;
        }
        public object GetByUserId(int pageNumber, int count, string? searchCommand)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            searchCommand = searchCommand ?? "";
            var dbShops = _shopRepository.GetQuery().Where(z =>z.UserId == userId &&( z.ShopName.Contains(searchCommand) || z.TaxUnit.Contains(searchCommand)
                || z.TerminalNumber.Contains(searchCommand) || z.Address.Contains(searchCommand) || z.FileNumber.Contains(searchCommand)
                || z.NationalId.Contains(searchCommand))).ToList();

            var dbCount = dbShops.Count();

            if (pageNumber != 0 && count != 0)
            {
                dbShops = dbShops.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            var result = new
            {
                Shops = dbShops,
                Count = dbCount
            };
            return result;
        }
        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbShop = _shopRepository.GetQuery().FirstOrDefault(z => z.ShopId == id);
                if (dbShop != null)
                {
                    await _shopRepository.Delete(dbShop);
                    return true;
                }
                else return false;
            }
            catch (Exception ex) { return false; }
        }
    }
}
