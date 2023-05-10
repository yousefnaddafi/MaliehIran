using MaliehIran.Extensions;
using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.CryptoAccountServices
{
    public class CryptoAccountService : ICryptoAccountService
    {
        private readonly IProjectEFRepository<CryptoAccount> cryptoAccountRepository;
        private readonly IHttpContextAccessor _accessor;
        public CryptoAccountService(IProjectEFRepository<CryptoAccount> cryptoAccountRepository,IHttpContextAccessor accessor)
        {
            this.cryptoAccountRepository = cryptoAccountRepository;
            _accessor = accessor;
        }

        public long Create(CryptoAccount model)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            model.IsDeleted = false;
            model.CreateDate = DateTime.Now;
            model.UserId = userId;
            var account = cryptoAccountRepository.Insert(model);
            return account.CryptoAccountId;

        }
        public async Task<CryptoAccount> Update(CryptoAccount model)
        {
            try
            {
                var account = await cryptoAccountRepository.Update(model);
                return account;
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
                var dbaccount = cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == id);
                if (dbaccount != null)
                {
                    await cryptoAccountRepository.Delete(dbaccount);
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

        public CryptoAccount Get(long id)
        {
            return cryptoAccountRepository.GetQuery().FirstOrDefault(z => z.CryptoAccountId == id);
        }

        public object GetAll(int pageNumber, int count,ExchangeType? exchange)
        {
            var accounts = cryptoAccountRepository.GetQuery();
            
            if (exchange != null)
            {
                accounts = accounts.Where(z => z.Exchange == exchange);
            }
            
            var accountsCount = accounts.Count();
            if (pageNumber != 0 && count != 0)
            {
                accounts = accounts.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count);
            }
            var DbAccounts = accounts.ToList();
            var result = new
            {
                Accounts = DbAccounts,
                AccountsCount = accountsCount
            };
            return result;
        }
        public object GetByUserId(int pageNumber, int count, ExchangeType? exchange)
        {
            var userId = _accessor.HttpContext.User.Identity.IsAuthenticated ?
                _accessor.HttpContext.User.Identity.GetUserId() : 0;
            var accounts = cryptoAccountRepository.GetQuery().Where(z => z.UserId == userId);
            if (exchange != null)
            {
                accounts = accounts.Where(z => z.Exchange == exchange);
            }

            var accountsCount = accounts.Count();
            if (pageNumber != 0 && count != 0)
            {
                accounts = accounts.OrderByDescending(z => z.CreateDate).Skip((pageNumber - 1) * count).Take(count);
            }
            var DbAccounts = accounts.ToList();
            var result = new
            {
                Accounts = DbAccounts,
                AccountsCount = accountsCount
            };
            return result;
        }
    }
}
