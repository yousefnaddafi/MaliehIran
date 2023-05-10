using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.MarketServices
{
    public interface IMarketService
    {
        long Create(Market model);
        Task<Market> Update(Market model);
        Task<bool> Delete(long id);
        Market Get(long id);
        object GetAll(int pageNumber, int count, string searchCommand);
    }
}
