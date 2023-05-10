using MaliehIran.Infrastructure;
using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.CronServices
{
    public class CronService : ICronService
    {
        private readonly IProjectEFRepository<Cron> cronRepository;
        public CronService(IProjectEFRepository<Cron> cronRepository)
        {
            this.cronRepository= cronRepository;
        }

        public long Create(Cron model)
        {
            model.IsDeleted = false;
            model.CreateDate = DateTime.Now;
            var dbCron = cronRepository.Insert(model);
            return dbCron.CronId;

        }
        public async Task<Cron> Update(Cron model)
        {
            try
            {
                var dbCron = await cronRepository.Update(model);
                return dbCron;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> Delete(long id)
        {
            try
            {
                var dbCron = cronRepository.GetQuery().FirstOrDefault(z => z.CronId == id);
                if (dbCron != null)
                {
                    await cronRepository.Delete(dbCron);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Cron Get(long id)
        {
            return cronRepository.GetQuery().FirstOrDefault(z => z.CronId == id);
        }

        public object GetAll(int pageNumber, int count)
        {
            var crons = cronRepository.GetQuery();
            var cronsCount = crons.Count();
            if (pageNumber != 0 && count != 0)
            {
                crons = crons.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count);
            }
            var dbCrons = crons.ToList();
            var result = new
            {
                Crons = dbCrons,
                CronsCount = cronsCount
            };
            return result;
        }
    }
}
