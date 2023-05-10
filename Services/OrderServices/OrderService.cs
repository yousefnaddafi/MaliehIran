using MaliehIran.Extensions;
using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.CoinexServices;
using MaliehIran.Services.KucoinServices;
using Kucoin.Net.Clients;
using Kucoin.Net.Enums;
using Kucoin.Net.Objects.Models.Spot;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OrderStatus = MaliehIran.Models.Enums.OrderStatus;

namespace MaliehIran.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IProjectEFRepository<Order> orderRepository;
        private readonly IProjectEFRepository<User> userRepository;
        private readonly ICoinexService coinexService;
        private readonly IKuCoinService kuCoinService;
        private readonly IHttpContextAccessor _accessor;
        public OrderService(IProjectEFRepository<Order> orderRepository, IHttpContextAccessor accessor,
            ICoinexService coinexService,IKuCoinService kuCoinService,IProjectEFRepository<User> userRepository)
        {
            this.orderRepository = orderRepository;
            _accessor = accessor;
            this.kuCoinService = kuCoinService;
            this.coinexService = coinexService;
            this.userRepository = userRepository;
        }

        public long Create(Order model)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            model.IsDeleted = false;
            model.CreateDate = DateTime.Now;
            model.BlockchainOrderId = "";
            model.Status = OrderStatus.Run;
            model.UserId = userId;
            var dbOrder = orderRepository.Insert(model);
            return dbOrder.OrderId;

        }
        public async Task<Order> Update(Order model)
        {
            try
            {
                var dbOrder = await orderRepository.Update(model);
                return dbOrder;
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
                var dbOrder = orderRepository.GetQuery().FirstOrDefault(z => z.OrderId == id);
                if (dbOrder != null)
                {
                    await orderRepository.Delete(dbOrder);
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

        public Order Get(long id)
        {
            return orderRepository.GetQuery().FirstOrDefault(z => z.OrderId == id);
        }

        public object GetAll(int pageNumber, int count,OrderStatus? status,ExchangeType? exchange,TradeTypes? tradeType)
        {
            var orders = orderRepository.GetQuery();
            if(status != null)
            {
                orders = orders.Where(z => z.Status == status);
            }
            if(exchange != null)
            {
                orders = orders.Where(z => z.Exchange == exchange);
            }
            if(tradeType != null)
            {
                orders = orders.Where(z => z.Type == tradeType);
            }
            var ordersCount = orders.Count();
            if (pageNumber != 0 && count != 0)
            {
                orders = orders.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count);
            }
            var dbOrders = orders.ToList();
            var result = new
            {
                Orders = dbOrders,
                OrdersCount = ordersCount
            };
            return result;
        }
        public object GetByUserId(int pageNumber, int count, OrderStatus? status, ExchangeType? exchange, TradeTypes? tradeType)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var orders = orderRepository.GetQuery().Where(z=>z.UserId == userId);
            if (status != null)
            {
                orders = orders.Where(z => z.Status == status);
            }
            if (exchange != null)
            {
                orders = orders.Where(z => z.Exchange == exchange);
            }
            if (tradeType != null)
            {
                orders = orders.Where(z => z.Type == tradeType);
            }
            var ordersCount = orders.Count();
            if (pageNumber != 0 && count != 0)
            {
                orders = orders.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count);
            }
            var dbOrders = orders.ToList();
            var result = new
            {
                Orders = dbOrders,
                OrdersCount = ordersCount
            };
            return result;
        }
        public async Task<object> ChangeOrderStatus(long orderId , OrderStatus status)
        {
            var order = orderRepository.GetQuery().FirstOrDefault(z => z.OrderId == orderId);
            if(order != null)
            {
                order.Status = status;
                if(status == OrderStatus.Cancle)
                {
                    var type = "buy";
                    if(order.Type == TradeTypes.sell)
                    {
                        type = "sell";
                    }
                    try
                    {
                        if(!string.IsNullOrEmpty(order.BlockchainOrderId) && order.BlockchainOrderId != "0")
                        if(order.Exchange == ExchangeType.Coinex)
                        {
                            coinexService.CancleOrder(Convert.ToInt64(order.BlockchainOrderId), order.Market + "USDT", type, order.CryptoAccountId);
                            order.Status = OrderStatus.Cancled;
                        }
                        else
                        {
                            kuCoinService.KucoinCancleOrder(order.CryptoAccountId, order.BlockchainOrderId);
                            order.Status = OrderStatus.Cancled;
                        }
                    }
                    catch(Exception ex)
                    {
                        return false;
                    }
                    
                }
                
                
                await orderRepository.Update(order);
                return true;
            }
            else
            {
                return false;
            }
        }


        public void OrdersCheck()
        {
            var dbActiveOrders = orderRepository.GetQuery().Where(z => z.Status == OrderStatus.Run || z.Status == OrderStatus.Cancle).ToList();
            foreach(var order in dbActiveOrders)
            {
                if(order.Exchange == ExchangeType.Coinex)
                {
                    CheckCoinex(order);
                }
                if(order.Exchange == ExchangeType.Kucoin)
                {
                    CheckKucoin(order);
                }
            }
        }

        public void CheckCoinex(Order order)
        {
            try
            {
                var market = order.Market + "USDT";
                var type = "buy";
                if (order.Type == TradeTypes.sell)
                {
                    type = "sell";
                }

                if (order.Status == OrderStatus.Cancle)
                {
                    if (!string.IsNullOrEmpty(order.BlockchainOrderId) && order.BlockchainOrderId != "0")
                    {
                        try
                        {
                            coinexService.CancleOrder(Convert.ToInt64(order.BlockchainOrderId), market, type, order.CryptoAccountId);
                            order.Status = OrderStatus.Cancled;
                            var cancled = orderRepository.Update(order).Result;
                        }
                        catch(Exception ex)
                        {
                        }

                    }
                    else
                    {
                        order.Status = OrderStatus.Cancled;
                        var cancled = orderRepository.Update(order).Result;
                    }
                }
                else
                {
                    var KandelsList = new List<Candle>();
                    var Kans = coinexService.AcquireKLineData(market, order.Length + 1, order.TimeFrame,order.CryptoAccountId).ToString();
                    var desersial = JsonSerializer.Deserialize<CandleApiResult>(Kans);
                    foreach (var item in desersial.data)
                    {
                        var k = new Candle
                        {
                            Time = item[0].ToString(),
                            Open = item[1].ToString(),
                            Close = item[2].ToString(),
                            High = item[3].ToString(),
                            Low = item[4].ToString(),
                            Data1 = item[5].ToString(),
                            Data2 = item[6].ToString()
                        };
                        KandelsList.Add(k);
                    }
                    var MethodResult = CalculateAverage(order.Method, KandelsList.Take(order.Length).ToList());
                    if (order.Type == TradeTypes.buy)
                    {
                        MethodResult -= order.RiskAversion * MethodResult / 100;
                    }
                    else
                    {
                        MethodResult += order.RiskAversion * MethodResult / 100;
                    }
                    var roundTo = KandelsList.FirstOrDefault().Open.Split('.')[1].Length;
                    var optimizedOrderPrice = Math.Round(MethodResult, roundTo);

                    if (optimizedOrderPrice != order.MainOrderPrice)
                    {
                        if (!string.IsNullOrEmpty(order.BlockchainOrderId) && order.BlockchainOrderId != "0")
                        {
                            var orderId = Convert.ToInt64(order.BlockchainOrderId);

                            coinexService.CancleOrder(orderId, market, type, order.CryptoAccountId);
                        }
                        string currentAmount = "";
                        if (order.Type == TradeTypes.sell && order.Percent != null && order.Percent > 0)
                        {
                            currentAmount = CalculateSellPercent((decimal)order.Percent, market, order.CryptoAccountId).ToString();
                        }
                        else
                        {
                            currentAmount = order.Amount;
                        }
                        if (order.Type == TradeTypes.buy)
                        {
                            decimal dollarAmount = 0;
                            if (order.Percent != null && order.Percent > 0)
                            {
                                dollarAmount = CalculateBuyPercent((decimal)order.Percent, order.CryptoAccountId);
                            }
                            else
                            {
                                dollarAmount = Convert.ToDecimal(order.Amount);
                            }
                            currentAmount = CalculateBuyAmount(dollarAmount, optimizedOrderPrice, market).ToString();
                        }

                        var OrderSetResult = coinexService.PlaceLimitOrder(market, type, currentAmount, optimizedOrderPrice.ToString(), order.CryptoAccountId).ToString();
                        var ApiDeserialize = JsonSerializer.Deserialize<OrderApiResult>(OrderSetResult);
                        order.BlockchainOrderId = ApiDeserialize.data.id.ToString();
                        order.MainOrderPrice = optimizedOrderPrice;

                        var Accountinfo = coinexService.AccountInfo(order.CryptoAccountId).ToString();
                        var AccountDeserialized = JsonSerializer.Deserialize<AccountApiResult>(Accountinfo);
                    }
                    else
                    {
                        var orderId = Convert.ToInt64(order.BlockchainOrderId);
                        var orderStatus = coinexService.GetOrderStatus(market, orderId, order.CryptoAccountId).ToString();
                        var deserializedOrderStatus = JsonSerializer.Deserialize<OrderApiResult>(orderStatus);

                        if (deserializedOrderStatus.data.left == "0")
                        {
                            order.Status = OrderStatus.End;
                            var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == order.UserId);
                            if (dbUser != null && !string.IsNullOrEmpty(dbUser.Email))
                            {
                                SendMessage($"Your Order of {order.Market} Crypto with orderId = {order.BlockchainOrderId} Is completed !!!", dbUser.Email);
                            }
                        }
                    }
                    
                }
                var updtOrder = orderRepository.Update(order).Result;
            }
            catch(Exception ex)
            {
                
            }
        }
        public void CheckKucoin(Order order)
        {
            try
            {
                if (order.Status == OrderStatus.Cancle)
                {
                    if (!string.IsNullOrEmpty(order.BlockchainOrderId) && order.BlockchainOrderId != "0")
                    {
                        try
                        {
                            kuCoinService.KucoinCancleOrder(order.CryptoAccountId,order.BlockchainOrderId);
                            order.Status = OrderStatus.Cancled;
                            var cancled = orderRepository.Update(order).Result;
                        }
                        catch
                        {
                        }

                    }
                    else
                    {
                        order.Status = OrderStatus.Cancled;
                        var cancled = orderRepository.Update(order).Result;
                    }
                }
                else
                {
                    var timeFrameNew = order.TimeFrame;
                    var FinalTimeFrame = "60";
                    switch (timeFrameNew)
                    {
                        case ("1min"):
                            FinalTimeFrame = "60";
                            break;
                        case ("3min"):
                            FinalTimeFrame = "180";
                            break;
                        case ("5min"):
                            FinalTimeFrame = "300";
                            break;
                        case ("15min"):
                            FinalTimeFrame = "900";
                            break;
                        case ("30min"):
                            FinalTimeFrame = "1800";
                            break;
                        case ("1hour"):
                            FinalTimeFrame = "3600";
                            break;
                        case ("2hour"):
                            FinalTimeFrame = "7200";
                            break;
                        case ("4hour"):
                            FinalTimeFrame = "14400";
                            break;
                        case ("6hour"):
                            FinalTimeFrame = "21600";
                            break;
                        case ("8hour"):
                            FinalTimeFrame = "28800";
                            break;
                        case ("12hour"):
                            FinalTimeFrame = "43200";
                            break;
                        case ("1day"):
                            FinalTimeFrame = "86400";
                            break;
                        case ("1week"):
                            FinalTimeFrame = "604800";
                            break;
                    }
                    var kandel = order.Length;
                    var market = order.Market;
                    KlineInterval timeFrame = (KlineInterval)Enum.Parse(typeof(KlineInterval), FinalTimeFrame);
                    var methods = order.Method;
                    var UPercent = order.RiskAversion;
                    var Percent = order.Percent;
                    if (order.Percent == null)
                    {
                        Percent = null;
                    }

                    var type = "buy";
                    if (order.Type == TradeTypes.sell)
                    {
                        type = "sell";
                    }

                    var amount = order.Amount;

                    var KlinesForRound = kuCoinService.KucoinGetKlines(market + "-USDT", timeFrame, order.CryptoAccountId).First();
                    var roundTo = KucoinGetRound(KlinesForRound);
                    var KandelsList = new List<Candle>();
                    var KKans = kuCoinService.KucoinGetKlines(market + "-USDT", timeFrame, order.CryptoAccountId);
                    KKans.RemoveAt(0);
                    foreach (var item in KKans)
                    {
                        var k = new Candle
                        {
                            Time = item.OpenTime.ToString(),
                            Open = item.OpenPrice.ToString(),
                            Close = item.ClosePrice.ToString(),
                            High = item.HighPrice.ToString(),
                            Low = item.LowPrice.ToString(),
                            Data1 = item.QuoteVolume.ToString(),
                            Data2 = item.Volume.ToString()
                        };
                        KandelsList.Add(k);
                    }
                    var MethodResult = CalculateAverage(methods, KandelsList.Take(kandel).ToList());
                    if (type == "buy")
                    {
                        MethodResult -= UPercent * MethodResult / 100;
                    }
                    else
                    {
                        MethodResult += UPercent * MethodResult / 100;
                    }

                    //var roundTo = KandelsList.FirstOrDefault().Open.Split('.')[1].Length;
                    var optimizedOrderPrice = Math.Round(MethodResult, roundTo);

                    if (optimizedOrderPrice != order.MainOrderPrice)
                    {
                        if (!string.IsNullOrEmpty(order.BlockchainOrderId))
                        {
                            var State = kuCoinService.KucoinGetOrder(order.CryptoAccountId, order.BlockchainOrderId);
                            Percent = Percent * (((decimal)State.Quantity - State.QuantityFilled) / (decimal)State.Quantity);
                            amount = (Convert.ToDecimal(amount) * (((decimal)State.Quantity - State.QuantityFilled) / (decimal)State.Quantity)).ToString();
                            kuCoinService.KucoinCancleOrder(order.CryptoAccountId, order.BlockchainOrderId);
                        }
                        string currentAmount = "";
                        if (type == "sell" && Percent != null && Percent > 0)
                        {
                            currentAmount = KucoinCalculateSellPercent((decimal)Percent, market, order.CryptoAccountId).ToString();
                        }
                        else
                        {
                            currentAmount = amount;
                        }
                        if (type == "buy")
                        {
                            decimal dollarAmount = 0;
                            if (Percent != null && Percent > 0)
                            {
                                dollarAmount = KucoinCalculateBuyPercent((decimal)Percent, order.CryptoAccountId);
                            }
                            else
                            {
                                dollarAmount = Convert.ToDecimal(amount);
                            }
                            currentAmount = KucoinCalculateBuyAmount(dollarAmount, optimizedOrderPrice, market, order.CryptoAccountId).ToString();

                        }
                        var kucoinAmount = Convert.ToDecimal(currentAmount);
                        var side = type == "buy" ? OrderSide.Buy : OrderSide.Sell;
                        var OrderSetResult = kuCoinService.KucoinPlaceOrderLimit(order.CryptoAccountId, market.ToUpper() + "-USDT", side, kucoinAmount, optimizedOrderPrice);
                        order.BlockchainOrderId = OrderSetResult;
                        order.MainOrderPrice = optimizedOrderPrice;

                    }
                    else
                    {
                        var State = kuCoinService.KucoinGetOrder(order.CryptoAccountId, order.BlockchainOrderId);
                        if (State != null)
                        {
                            if (State.QuantityFilled == State.Quantity)
                            {
                                order.Status = OrderStatus.End;
                                var dbUser = userRepository.GetQuery().FirstOrDefault(z => z.UserId == order.UserId);
                                if (dbUser != null && !string.IsNullOrEmpty(dbUser.Email))
                                {
                                    SendMessage($"Your Order of {order.Market} Crypto with orderId = {order.BlockchainOrderId} Is completed !!!", dbUser.Email);
                                }
                            }
                        }

                    }
                    var update = orderRepository.Update(order).Result;
                }


                
            }
            catch (Exception ex)
            {
                
            }
        }

        public decimal CalculateAverage(int method, List<Candle> candles)
        {
            decimal result = 0;
            switch ((int)method)
            {
                case (1):
                    result = candles.Select(z => Convert.ToDecimal(z.Open)).Average();
                    break;
                case (2):
                    result = candles.Select(z => Convert.ToDecimal(z.Close)).Average();
                    break;
                case (3):
                    result = candles.Select(z => Convert.ToDecimal(z.High)).Average();
                    break;
                case (4):
                    result = candles.Select(z => Convert.ToDecimal(z.Low)).Average();
                    break;
                case (5):
                    result = candles.Select(z => (Convert.ToDecimal(z.High) + Convert.ToDecimal(z.Low)) / 2).Average();
                    break;
                case (6):
                    result = candles.Select(z => (Convert.ToDecimal(z.Open) + Convert.ToDecimal(z.Close)) / 2).Average();
                    break;
                case (7):
                    result = candles.Select(z => (Convert.ToDecimal(z.Open) + Convert.ToDecimal(z.Close) + Convert.ToDecimal(z.High) + Convert.ToDecimal(z.Low)) / 4).Average();
                    break;
            }
            return result;
        }
        public decimal CalculateSellPercent(decimal percent, string market,long cryptoAccountId)
        {
            var accountInfo = coinexService.AccountInfo(cryptoAccountId).ToString();
            var AccountDeserialized = JsonSerializer.Deserialize<AccountApiResult>(accountInfo);
            var Frozen = AccountDeserialized.data.FirstOrDefault(z => market.ToUpper().Contains(z.Key)).Value.frozen;
            var Available = AccountDeserialized.data.FirstOrDefault(z => market.ToUpper().Contains(z.Key)).Value.available;
            var avail = Convert.ToDecimal(Available);
            var result = (avail * percent) / 100;
            return result;
        }
        public decimal CalculateBuyPercent(decimal percent,long cryptoAccountId)
        {
            var accountInfo = coinexService.AccountInfo(cryptoAccountId).ToString();
            var AccountDeserialized = JsonSerializer.Deserialize<AccountApiResult>(accountInfo);
            var Frozen = AccountDeserialized.data.FirstOrDefault(z => "USDT".Contains(z.Key)).Value.frozen;
            var Available = AccountDeserialized.data.FirstOrDefault(z => "USDT".Contains(z.Key)).Value.available;
            var avail = Convert.ToDecimal(Available);
            var result = (avail * percent) / 100;
            return result;
        }
        public decimal CalculateBuyAmount(decimal Dollar, decimal optimizedPrice, string market)
        {
            var MarketInfo = coinexService.AcquireSingleMarketInfo(market).ToString();
            var result = Dollar / optimizedPrice;
            var marketSerialized = JsonSerializer.Deserialize<MarketInfoResult>(MarketInfo);
            var round = marketSerialized.data.trading_decimal;
            var finalResult = Math.Round(result, (int)round);
            return finalResult;
        }

        public int KucoinGetRound(KucoinKline input)
        {
            var open = input.OpenPrice.ToString().Contains(".") ? input.OpenPrice.ToString().Split(".")[1].Length : 0;
            var close = input.OpenPrice.ToString().Contains(".") ? input.OpenPrice.ToString().Split(".")[1].Length : 0;
            var low = input.OpenPrice.ToString().Contains(".") ? input.OpenPrice.ToString().Split(".")[1].Length : 0;
            var high = input.OpenPrice.ToString().Contains(".") ? input.OpenPrice.ToString().Split(".")[1].Length : 0;
            var LOCLH = new List<int>();
            LOCLH.Add(open);
            LOCLH.Add(close);
            LOCLH.Add(high);
            LOCLH.Add(low);
            return LOCLH.Max();
        }

        public decimal KucoinCalculateSellPercent(decimal percent, string market,long cryptoAccountId)
        {
            var accountInfo = kuCoinService.KucoinListAccounts(cryptoAccountId);
            var TheAccount = accountInfo.FirstOrDefault(z => z.Asset == market.ToUpper());
            var Frozen = TheAccount.Holds;
            var Available = TheAccount.Available;
            var avail = Convert.ToDecimal(Available);
            var result = (avail * percent) / 100;
            return result;
        }

        public decimal KucoinCalculateBuyPercent(decimal percent,long cryptoAccountId)
        {
            var accountInfo = kuCoinService.KucoinListAccounts(cryptoAccountId);
            var TheAccount = accountInfo.FirstOrDefault(z => z.Asset == "USDT");
            var Frozen = TheAccount.Holds;
            var Available = TheAccount.Available;
            var avail = Convert.ToDecimal(Available);
            var result = (avail * percent) / 100;
            return result;
        }

        public decimal KucoinCalculateBuyAmount(decimal Dollar, decimal optimizedPrice, string symbol , long cryptoAccountId)
        {
            var result = Dollar / optimizedPrice;
            var symbolData = kuCoinService.KucoinGetSymbolList(symbol, cryptoAccountId);
            var roundTo = symbolData.BaseIncrement.ToString().Contains(".") ?
                symbolData.BaseIncrement.ToString().Split(".")[1].Length : 0;
            var finalResult = Math.Round(result, roundTo);
            return finalResult;
        }

        

        #region SMS
        public void SendMessage(string messageBody,string email)
        {
            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Herbod Crypto",
            "herbodfisherbot@gmail.com");
            message.From.Add(from);
            MailboxAddress to = new MailboxAddress("User",
            $"{email}");
            message.To.Add(to);
            message.Subject = "Order Status";

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<h1>Hello Dear Subscriber !</h1> <br/> <p>{messageBody}</p>";
            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate("herbodfisherbot@gmail.com", "typaxyklzsolcihv");

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
        #endregion
    }
}
