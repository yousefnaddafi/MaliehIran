using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.AccountServices;
using MaliehIran.Services.Common;
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
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        [HttpPost]
        public async Task<IServiceResult<object>> SignIn(UserSignDTO model)
        {
            return await accountService.SignIn(model);
        }
        [HttpPost]
        public async Task<IServiceResult<object>> SignUp(UserSignDTO model)
        {
            return await accountService.SignUp(model);
        }
        [HttpPost]
        public async Task<IServiceResult<User>> GetById(int userId)
        {
            return await accountService.GetById(userId);
        }
        [HttpPost]
        public async Task<IServiceResult<object>> SendConfirmationCodeToBoth(string mobile,string Email)
        {
            return await accountService.SendConfirmationCodeToBoth(mobile,Email);
        }
        [HttpPost]
        public async Task<IServiceResult<User>> UpdateUser(User user)
        {
            return await accountService.UpdateUser(user);
        }
        [HttpPost]
        public async Task<IServiceResult<object>> ForgotPassword(AuthTypes type, string PhoneOrEmail)
        {
            return await accountService.ForgotPassword(type,PhoneOrEmail);
        }

        [HttpPost]
        public async Task<IServiceResult<object>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            return await accountService.ChangePassword(changePasswordDto);
        }
    }
}
