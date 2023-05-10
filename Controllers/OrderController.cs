using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.OrderServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpPost]
        public long Create(Order model)
        {
            return orderService.Create(model);
        }

        [HttpPost]
        public async Task<Order> Update(Order model)
        {
            return await orderService.Update(model);
        }

        [HttpPost]
        public async Task<bool> Delete(long id)
        {
            return await orderService.Delete(id);
        }

        [HttpPost]
        public Order Get(long id)
        {
            return orderService.Get(id);
        }

        [HttpPost]
        public object GetAll(int pageNumber, int count, OrderStatus? status, ExchangeType? exchange, TradeTypes? tradeType)
        {
            return orderService.GetAll(pageNumber,count,status,exchange,tradeType);
        }

        [HttpPost]
        public object GetByUserId(int pageNumber, int count, OrderStatus? status, ExchangeType? exchange, TradeTypes? tradeType)
        {
            return orderService.GetByUserId(pageNumber, count, status, exchange, tradeType);
        }
        [HttpPost]
        public async Task<object> ChangeOrderStatus(long orderId, OrderStatus status)
        {
            return await orderService.ChangeOrderStatus(orderId,status);
        }
        
    }
}
