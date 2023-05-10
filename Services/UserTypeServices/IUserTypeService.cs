using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.UserTypeServices
{
    public interface IUserTypeService
    {
        Task<long> Create(UserType inputDto);
        Task<UserType> Update(UserType item);
        Task<bool> Delete(long id);
        Task<List<UserType>> GetAll();
        Task<UserType> Get(long id);
        List<UserType> GetByCommand(string sql);
        long UpdateByCmd(string name, long id);
    }
}
