using MaliehIran.Models;
using MaliehIran.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.UserServices
{
    public interface IUserService
    {
        Task<long> Create(User inputDto);
        Task<IServiceResult<User>> Update(User item);
        Task<bool> Delete(long id);
        Task<object> GetAll(int pageNumber, int count, string searchCommand);
        Task<object> ChangeUserRole(long userId, long userRole);
        User Get(long id);
        User GetById();
    }
}
