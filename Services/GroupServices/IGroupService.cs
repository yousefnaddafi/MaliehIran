using MaliehIran.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaliehIran.Services.GroupServices
{
    public interface IGroupService
    {
        Task Create(Group inputDto);
        Task<Group> Update(Group item);
        Task Delete(long id);
        Task<List<Group>> GetAll();
        Task<Group> Get(long id);
    }
}
