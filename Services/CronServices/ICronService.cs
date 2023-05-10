using MaliehIran.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.CronServices
{
    public interface ICronService
    {
        long Create(Cron model);
        Task<Cron> Update(Cron model);
        Task<bool> Delete(long id);
        Cron Get(long id);
        object GetAll(int pageNumber, int count);
    }
}
