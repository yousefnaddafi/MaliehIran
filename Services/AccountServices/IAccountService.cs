using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.AccountServices
{
    public interface IAccountService
    {
        Task<IServiceResult<object>> SignIn(UserSignDTO model);
        Task<IServiceResult<object>> SignUp(UserSignDTO model);
        Task<IServiceResult<User>> GetById(int userId);
        Task<IServiceResult<List<User>>> GetByIds(List<int> ids);
        //Task<IServiceResult<object>> SendConfirmationCode(string phoneNumber);
        Task<IServiceResult<object>> SendConfirmationCodeToEmail(string Email);
        Task<IServiceResult<SignStatus>> ValidateConfirmationCode(string phoneNumber, string code);
        //Task<IServiceResult<User>> CreateUser(User user);
        Task<IServiceResult<User>> UpdateUser(User user);
        Task<IServiceResult> DeleteUser(int userId);
        //Task<IServiceResult> ResetPassord(User user);
        Task<bool> CheckUserName(string userName);
        //Task<IServiceResult> RemoveUser(long userId);
        Task<IServiceResult<object>> ForgotPassword(AuthTypes type, string PhoneOrEmail);
        Task<IServiceResult<object>> ChangePassword(ChangePasswordDto changePasswordDto);
    }
}
