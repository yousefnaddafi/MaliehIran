using MaliehIran.Infrastructure;
using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.MarketServices
{
    public class MarketService : IMarketService
    {
        private readonly IProjectEFRepository<Market> marketRepository;
        public MarketService(IProjectEFRepository<Market> marketRepository)
        {
            this.marketRepository = marketRepository;
        }

        public long Create(Market model)
        {
            model.IsDeleted = false;
            model.CreateDate = DateTime.Now;
            var dbMarket = marketRepository.GetQuery().FirstOrDefault(z => z.MarketName.Contains(model.MarketName));
            if(dbMarket == null)
            {
                dbMarket = marketRepository.Insert(model);
                return dbMarket.MarketId;
            }
            return 0;
        }
        public async Task<Market> Update(Market model)
        {
            try
            {
                var dbMarket = await marketRepository.Update(model);
                return dbMarket;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbMarket = marketRepository.GetQuery().FirstOrDefault(z => z.MarketId == id);
                if(dbMarket != null)
                {
                    await marketRepository.Delete(dbMarket);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public Market Get(long id)
        {
            return marketRepository.GetQuery().FirstOrDefault(z => z.MarketId == id);
        }

        public object GetAll(int pageNumber,int count, string searchCommand)
        {
            searchCommand = searchCommand ?? "";
            var markets = marketRepository.GetQuery().Where(z => z.MarketName.Contains(searchCommand));
            var marketsCount = markets.Count();
            if(pageNumber != 0 && count != 0)
            {
                markets = markets.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count);
            }
            var dbMarkets = markets.ToList();
            var result = new
            {
                Markets = dbMarkets,
                MarketsCount = marketsCount
            };
            return result;
        }
    }
}
