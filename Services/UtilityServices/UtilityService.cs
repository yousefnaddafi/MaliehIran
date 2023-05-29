using MaliehIran.Infrastructure;
using MaliehIran.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MaliehIran.Models.Enums;
using MaliehIran.Extensions;

namespace MaliehIran.Services.UtilityServices
{
    public class UtilityService : IUtilityService
    {
        private readonly IProjectEFRepository<Utility> _utilityRepository;
        private readonly IHttpContextAccessor _accessor;
        public UtilityService(IProjectEFRepository<Utility> utilityRepository, IHttpContextAccessor accessor)
        {
            _utilityRepository = utilityRepository;
            _accessor = accessor;
        }
        public long Create(Utility model)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            model.UserId = userId;
            model.CreateDate = DateTime.Now;
            var creation = _utilityRepository.Insert(model);
            return creation.UtilityId;
        }
        public async Task<Utility> Update(Utility model)
        {
            return await _utilityRepository.Update(model);
        }
        public Utility Get(long id)
        {
            var dbUtility = _utilityRepository.GetQuery().FirstOrDefault(z => z.UtilityId == id);
            return dbUtility;
        }
        public object GetAll(int pageNumber, int count, long? userId, long? shopId, UtilityType? type, UtilityStatus? status)
        {
            var dbUtilities = _utilityRepository.GetQuery().ToList();

            if (shopId != null)
            {
                dbUtilities = dbUtilities.Where(z => z.ShopId == shopId).ToList();
            }
            if (userId != null)
            {
                dbUtilities = dbUtilities.Where(z => z.UserId == userId).ToList();
            }
            if (type != null)
            {
                dbUtilities = dbUtilities.Where(z => z.Type == type).ToList();
            }
            if (status != null)
            {
                dbUtilities = dbUtilities.Where(z => z.Status == status).ToList();
            }

            var dbCount = dbUtilities.Count();

            if (pageNumber != 0 && count != 0)
            {
                dbUtilities = dbUtilities.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            var result = new
            {
                Utilities = dbUtilities,
                Count = dbCount
            };
            return result;
        }
        public object GetByUserId(int pageNumber, int count,long? shopId, UtilityType? type, UtilityStatus? status)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var dbUtilities = _utilityRepository.GetQuery().Where(z => z.UserId == userId).ToList();
            if(shopId != null)
            {
                dbUtilities = dbUtilities.Where(z => z.ShopId == shopId).ToList();
            }
            if(type != null)
            {
                dbUtilities = dbUtilities.Where(z=>z.Type == type).ToList();
            }
            if (status != null)
            {
                dbUtilities = dbUtilities.Where(z=>z.Status == status).ToList();
            }

            var dbCount = dbUtilities.Count();

            if (pageNumber != 0 && count != 0)
            {
                dbUtilities = dbUtilities.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count).ToList();
            }
            var result = new
            {
                Utilities = dbUtilities,
                Count = dbCount
            };
            return result;
        }
        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbUtilities = _utilityRepository.GetQuery().FirstOrDefault(z => z.ShopId == id);
                if (dbUtilities != null)
                {
                    await _utilityRepository.Delete(dbUtilities);
                    return true;
                }
                else return false;
            }
            catch (Exception ex) { return false; }
        }
        public async Task<bool> ChangeStatus(long utilityId, UtilityStatus status)
        {
            try
            {
                var dbUtility = _utilityRepository.GetQuery().FirstOrDefault(z => z.UtilityId == utilityId);
                if (dbUtility != null)
                {
                    dbUtility.Status = status;
                    await _utilityRepository.Update(dbUtility);
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }
    }
}
