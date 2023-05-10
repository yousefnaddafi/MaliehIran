using MaliehIran.Models;
using MaliehIran.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.OrderServices
{
    public interface IOrderService
    {
        long Create(Order model);
        Task<Order> Update(Order model);
        Task<bool> Delete(long id);
        Order Get(long id);
        object GetAll(int pageNumber, int count, OrderStatus? status, ExchangeType? exchange, TradeTypes? tradeType);
        object GetByUserId(int pageNumber, int count, OrderStatus? status, ExchangeType? exchange, TradeTypes? tradeType);
        Task<object> ChangeOrderStatus(long orderId, OrderStatus status);
        void OrdersCheck();
    }
}
