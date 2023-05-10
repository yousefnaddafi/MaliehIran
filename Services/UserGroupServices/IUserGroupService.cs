using MaliehIran.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaliehIran.Services.UserGroupServices
{
    public interface IUserGroupService
    {
        Task Create(UserGroup inputDto);
        Task<UserGroup> Update(UserGroup item);
        Task Delete(long id);
        Task<List<UserGroup>> GetAll();
        Task<UserGroup> Get(long id);
        List<UserGroup> GetByUserId(long userId);
    }
}
