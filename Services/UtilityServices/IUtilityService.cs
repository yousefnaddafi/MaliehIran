using MaliehIran.Models.Enums;
using System.Threading.Tasks;
using System;
using MaliehIran.Models;

namespace MaliehIran.Services.UtilityServices
{
    public interface IUtilityService
    {
        long Create(Utility model);
        Task<Utility> Update(Utility model);
        Utility Get(long id);
        object GetAll(int pageNumber, int count, long? userId, long? shopId, UtilityType? type, UtilityStatus? status);
        object GetByUserId(int pageNumber, int count, long? shopId, UtilityType? type, UtilityStatus? status);
        Task<bool> Delete(long id);
        Task<bool> ChangeStatus(long utilityId, UtilityStatus status);
    }
}
