using MaliehIran.Services.OrderServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaliehIran.Services.BackGroundServices
{
    public class BackGroundService : IHostedService
    {
        private Timer? _timer = null;
        private readonly IOrderService orderService;
        public BackGroundService(IServiceScopeFactory factory )
        {
            orderService = factory.CreateScope().ServiceProvider.GetRequiredService<IOrderService>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //var interval = TimeSpan.FromMinutes(1);
            var interval = TimeSpan.FromMinutes(5);
            var UTCNOW = DateTime.UtcNow;
            var SecondsUTC = 60 - UTCNOW.Second;
            var miliSecondUTC = 1000 - UTCNOW.Millisecond;
            var minuteUTC =4 - UTCNOW.Minute % 5;
            //var nextRunTime = UTCNOW.AddMinutes(1);
            var nextRunTime = UTCNOW.AddMinutes(minuteUTC).AddSeconds(SecondsUTC+1).AddMilliseconds(miliSecondUTC);
            var curTime = UTCNOW;
            var firstInterval = nextRunTime.Subtract(curTime);

            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();
                //remove inactive accounts at expected time
                CheckOrders(null);
                //now schedule it to be called every 24 hours for future
                // timer repeates call to RemoveScheduledAccounts every 24 hours.
                _timer = new Timer(
                    CheckOrders,
                    null,
                    TimeSpan.Zero,
                    interval
                );
            };

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void CheckOrders(object state)
        {
            orderService.OrdersCheck();
        }
    }
}
