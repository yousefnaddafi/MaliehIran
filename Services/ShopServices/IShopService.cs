using MaliehIran.Models;
using System.Threading.Tasks;

namespace MaliehIran.Services.ShopServices
{
    public interface IShopService
    {
        long Create(Shop model);
        long CreateByAdmin(Shop model);
        Task<Shop> Update(Shop model);
        Shop Get(long id);
        object GetAll(int pageNumber, int count, long? userId, string? searchCommand);
        object GetByUserId(int pageNumber, int count, string? searchCommand);
        Task<bool> Delete(long id);
    }
}
