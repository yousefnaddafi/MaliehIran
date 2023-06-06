using MaliehIran.Infrastructure;
using MaliehIran.Models;
using MaliehIran.Models.Enums;
using MaliehIran.Services.Common;
using MaliehIran.Services.CryptographyServices;
using MaliehIran.Services.JWTServices;
using MaliehIran.Settings;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MaliehIran.Services.SMSServices;
using IPE.SmsIrClient.Models.Requests;
using IPE.SmsIrClient;

namespace MaliehIran.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly ISMSService _sMsService;
        private readonly IProjectEFRepository<User> _userRepository;

        private readonly IProjectEFRepository<UserType> _roleRepository;
        private readonly IEncryptService _encryptService;
        private readonly IGenerateJwtService _generateJwtService;
        private readonly IDistributedCache _cache;
        //private readonly ISmsSenderService _smsSenderService;
        private readonly ILogger<AccountService> _logger;
        private readonly IConfirmationCodeSetting _confirmationCodeSetting;
        private readonly IDecryptService _decryptService;
        private readonly IDistributedCache distributedCache;
        public AccountService(IEncryptService encryptService, IGenerateJwtService generateJwtService,
            IDistributedCache cache, ILogger<AccountService> logger, IDistributedCache distributedCache,
        IProjectEFRepository<UserType> roleRepository, IProjectEFRepository<User> userRepository,
            IConfirmationCodeSetting confirmationCodeSetting, ISMSService sMsService,IDecryptService decryptService)
        {
            _encryptService = encryptService;
            _generateJwtService = generateJwtService;
            _cache = cache;
            _sMsService = sMsService;
            _logger = logger;
            //_dateTime = dateTime;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _confirmationCodeSetting = confirmationCodeSetting;
            _decryptService = decryptService;
            this.distributedCache = distributedCache;
        }

        #region SignIn
        public async Task<IServiceResult<object>> SignInWithMobileOrEmail(long Type, string Mobile, string Mail, string Password, string VerifyCode)
        {
            var user = new User();
            if (Type == (int)AuthTypes.Mobile)
            {
                user = await _userRepository.GetQuery().Where(u => u.Mobile.Equals(Mobile)).FirstOrDefaultAsync();
            }
            if (Type == (int)AuthTypes.Email)
            {
                user = await _userRepository.GetQuery().Where(u => u.Email.Equals(Mail)).FirstOrDefaultAsync();
            }

            if (user == null)
            {
                var result = new
                {
                    Data = "",
                    Status = (int)SignStatus.NoUserFound
                };
                return new ServiceResult<object>().Ok(result);
            }

            if (user.Status != 1)
            {
                var result = new
                {
                    Data = "",
                    Status = (int)SignStatus.UserNotActive
                };
                return new ServiceResult<object>().Ok(result);
            }


            if (!string.IsNullOrEmpty(Mobile))
            {
                var validatePassword = false;
                var validateCode = false;

                if (!(user.Password is null) && !string.IsNullOrEmpty(Password))
                {
                    if (_encryptService.Encrypt(Password).Data == user.Password)
                        validatePassword = true;
                    else
                    {
                        var AAA = new
                        {
                            Data = "",
                            Status = (int)SignStatus.WrongUserPass
                        };
                        return new ServiceResult<object>().Ok(AAA);
                    }
                }

                if (!string.IsNullOrEmpty(VerifyCode))
                {
                    var passwordCache = await _cache.GetAsync(user.Mobile);
                    validateCode = passwordCache != null && VerifyCode.Equals(Encoding.UTF8.GetString(passwordCache));
                }

                if (validatePassword || validateCode)
                {
                    var jwtResult = await _generateJwtService.GenerateAsync(user);
                    if (jwtResult.IsSuccess)
                    {
                        await _cache.RemoveAsync(user.Mobile);
                        var BBB = new
                        {
                            Data = jwtResult.Data,
                            Status = (int)SignStatus.Success
                        };
                        return new ServiceResult<object>().Ok(BBB);
                    }
                }
                else
                {
                    var Obj = new
                    {
                        Data = "",
                        Status = (int)SignStatus.WrongUserPass
                    };
                    return new ServiceResult<object>().Ok(Obj);
                }
            }
            if (!string.IsNullOrEmpty(Mail))
            {
                var validatePassword = false;
                var validateCode = false;

                if (!(user.Password is null) && !string.IsNullOrEmpty(Password))
                {
                    if (_encryptService.Encrypt(Password).Data == user.Password)
                        validatePassword = true;
                    else
                    {
                        var AAA = new
                        {
                            Data = "",
                            Status = (int)SignStatus.WrongUserPass
                        };
                        return new ServiceResult<object>().Ok(AAA);
                    }
                }

                if (!string.IsNullOrEmpty(VerifyCode))
                {
                    var passwordCache = await _cache.GetAsync(user.Email);
                    validateCode = passwordCache != null && VerifyCode.Equals(Encoding.UTF8.GetString(passwordCache));
                }

                if (validatePassword || validateCode)
                {
                    var jwtResult = await _generateJwtService.GenerateAsync(user);
                    if (jwtResult.IsSuccess)
                    {
                        await _cache.RemoveAsync(user.Mobile);
                        var BBB = new
                        {
                            Data = jwtResult.Data,
                            Status = (int)SignStatus.Success
                        };
                        return new ServiceResult<object>().Ok(BBB);
                    }
                }
                else
                {
                    var Obj = new
                    {
                        Data = "",
                        Status = (int)SignStatus.WrongUserPass
                    };
                    return new ServiceResult<object>().Ok(Obj);
                }
            }

            var FFF = new
            {
                Data = "",
                Status = (int)SignStatus.WrongUserPass
            };
            return new ServiceResult<object>().Ok(FFF);
        }

        public async Task<IServiceResult<object>> SignInWithoutPassword(string encryptedMail)
        {
            var decriptContents = _decryptService.DecryptMail(encryptedMail).Data;
            var decriptContent = decriptContents.Split("///");
            var ReqTime = Convert.ToDateTime(decriptContent[1].Replace('/', ':'));
            var DecriptedMail = decriptContent[0].Replace('/', '@');
            var dateTimeNow = System.DateTime.UtcNow;
            if (dateTimeNow < ReqTime.AddSeconds(30))
            {
                var user = await _userRepository.GetQuery().Where(u => u.Mobile.ToUpper().Equals(DecriptedMail.ToUpper())).FirstOrDefaultAsync();
                if (!(user is null) && !string.IsNullOrEmpty(DecriptedMail))
                {
                    var jwtResult = await _generateJwtService.GenerateAsync(user);
                    if (jwtResult.IsSuccess)
                    {
                        await _cache.RemoveAsync(user.Mobile);
                        return new ServiceResult<object>().Ok(jwtResult.Data);
                    }
                }
                return new ServiceResult<object>().Ok(0);
            }
            else
            {
                return new ServiceResult<object>().Ok(0);
            }
        }

        public async Task<IServiceResult<object>> SignIn(UserSignDTO model)
        {
            switch (model.Type)
            {
                case (int)AuthTypes.Mobile:
                    var mobileResult = await SignInWithMobileOrEmail(model.Type, model.Mobile, model.Mail, model.Password, model.VerifyCode);
                    return new ServiceResult<object>().Ok(mobileResult.Data);
                case (int)AuthTypes.Email:
                    var mailResult = await SignInWithMobileOrEmail(model.Type, model.Mobile, model.Mail, model.Password, model.VerifyCode);
                    return new ServiceResult<object>().Ok(mailResult.Data);
                case (int)AuthTypes.WithoutPassword:
                    var result = await SignInWithoutPassword(model.EncryptedMail);
                    return new ServiceResult<object>().Ok(result.Data);
            }
            return new ServiceResult<object>().Ok(0);
        }

        #endregion

        #region SignUp
        public async Task<IServiceResult<object>> SignUpWithMobileAndMail(string firstname, string lastName, string mobile, string email, string code, long signUpType, string password, string userName, long UserRole, string plainPass)
        {
            if (UserRole == 2)
            {
                UserRole = 1;
            }
            var EmailUsers = new List<User>();
            var MobileUsers = new List<User>();
            if (!string.IsNullOrEmpty(mobile))
            {
                MobileUsers = await _userRepository.GetQuery().Where(user => user.Mobile.Equals(mobile)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(email))
            {
                EmailUsers = await _userRepository.GetQuery().Where(user => user.Email.Equals(email.ToLower())).ToListAsync();
            }

            if (!(MobileUsers.Any() || EmailUsers.Any()))
            {
                var confirmCode = new byte[] { };
                if (signUpType == (int)AuthTypes.Mobile)
                {
                    confirmCode = await _cache.GetAsync(mobile);
                }
                if (signUpType == (int)AuthTypes.Email)
                {
                    confirmCode = await _cache.GetAsync(email);
                }
                if (!(confirmCode is null))
                {
                    if (Encoding.UTF8.GetString(confirmCode) == code)
                    {
                        #region Old Insert
                        var user = new User()
                        {
                            Mobile = mobile,
                            Name = firstname,
                            FamilyName = lastName,
                            IsDeleted = false,
                            Email = email,
                            Password = password,
                            UserName = userName,
                            Status = 1,
                            Type = (await _roleRepository.GetQuery().Where(p => p.Name.Equals("Subscriber")).FirstOrDefaultAsync())
                                .UserTypeId
                        };
                        var userCreation = _userRepository.Insert(user);

                        #endregion

                        if (signUpType == (int)AuthTypes.Mobile)
                        {
                            _logger.LogInformation($"sign up ----> {mobile}");


                    //        #region send sms

                    //        UltraFastSend member = new UltraFastSend()
                    //        {
                    //            Mobile = Convert.ToInt64(mobile),
                    //            TemplateId = 66660,
                    //            ParameterArray = new List<UltraFastParameters>()
                    //{
                    //    new UltraFastParameters()
                    //    {
                    //        Parameter = "mobile" , ParameterValue = $"{mobile}"
                    //    },
                    //    new UltraFastParameters()
                    //    {
                    //        Parameter = "password" , ParameterValue = $"{plainPass}"
                    //    }
                    //}.ToArray()
                    //        };
                    //        var result = await _smsSenderService.UltraFastSend(member);

                    //        #endregion



                        }
                        if (signUpType == (int)AuthTypes.Email)
                        {
                            _logger.LogInformation($"sign up ----> {email}");
                        }


                        var jwtResult = await _generateJwtService.GenerateAsync(user);
                        if (jwtResult.IsSuccess)
                        {
                            await _cache.RemoveAsync(mobile);
                            await _cache.RemoveAsync(email);

                            var UserNames = _userRepository.GetQuery().Select(z => z.UserName).ToList();
                            var cacheKey = "UserNames";
                            var serializedCustomerList = JsonConvert.SerializeObject(UserNames);
                            var UserNameList = Encoding.UTF8.GetBytes(serializedCustomerList);
                            await distributedCache.SetAsync(cacheKey, UserNameList);


                            return new ServiceResult<object>().Ok(jwtResult.Data);
                        }
                        return new ServiceResult<object>().Failure(jwtResult.ErrorMessage);
                    }
                    return new ServiceResult<object>().Ok((int)SignStatus.WrongCode);
                }
                return new ServiceResult<object>().Ok((int)SignStatus.CodeExpire);
            }
            return new ServiceResult<object>().Ok((int)SignStatus.UserExists);
        }
        public async Task<IServiceResult<object>> SignUpWithoutCode(string firstname, string lastName, string encryptedMail)
        {
            var decriptContents = _decryptService.DecryptMail(encryptedMail).Data;
            var decriptContent = decriptContents.Split("///");
            var ReqTime = Convert.ToDateTime(decriptContent[1].Replace('/', ':'));
            var DecriptedMail = decriptContent[0].Replace('/', '@');
            var dateTimeNow = System.DateTime.UtcNow;
            if (dateTimeNow < ReqTime.AddSeconds(30))
            {
                var users = await _userRepository.GetQuery().Where(user => user.Mobile.Equals(DecriptedMail)).ToListAsync();
                if (!users.Any())
                {
                    var user = new User()
                    {
                        Mobile = DecriptedMail,
                        Name = firstname,
                        FamilyName = lastName,
                        Password = null,
                        IsDeleted = false,
                        Email = DecriptedMail,
                        Status = 1,
                        Type = (await _roleRepository.GetQuery().Where(p => p.Name.Equals("Subscriber")).FirstOrDefaultAsync())
                            .UserTypeId
                    };
                    await _userRepository.InsertAsync(user);
                    _logger.LogInformation($"sign up ----> {DecriptedMail}");

                    var jwtResult = await _generateJwtService.GenerateAsync(user);
                    if (jwtResult.IsSuccess)
                    {
                        await _cache.RemoveAsync(DecriptedMail);
                        return new ServiceResult<object>().Ok(jwtResult.Data);
                    }

                    return new ServiceResult<object>().Failure(jwtResult.ErrorMessage);
                }
                return new ServiceResult<object>().Failure("This email is already registered in the system");
            }
            else
            {
                return new ServiceResult<object>().Ok(0);
            }

        }
        public async Task<IServiceResult<object>> SignUp(UserSignDTO model)
        {
            var plainPass = model.Password;
            //Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$");
            Regex regex = new Regex(@"^.{4,12}$");
            Match match = regex.Match(model.Password);
            if (match.Success)
            {
                model.Password = _encryptService.Encrypt(model.Password).Data;
            }
            else
            {
                return new ServiceResult<object>().Ok((int)SignStatus.PassFormatNotMatched);
            }
            if (await CheckUserName(model.UserName))
            {
                switch (model.Type)
                {
                    case (int)AuthTypes.Mobile:
                        var mobileResult = await SignUpWithMobileAndMail(model.FirstName, model.LastName, model.Mobile, model.Mail, model.VerifyCode, model.Type, model.Password, model.UserName, model.UserRole, plainPass);
                        return new ServiceResult<object>().Ok(mobileResult.Data);
                    case (int)AuthTypes.Email:
                        var mailResult = await SignUpWithMobileAndMail(model.FirstName, model.LastName, model.Mobile, model.Mail, model.VerifyCode, model.Type, model.Password, model.UserName, model.UserRole, plainPass);
                        return new ServiceResult<object>().Ok(mailResult.Data);
                    case (int)AuthTypes.WithoutPassword:
                        var result = await SignUpWithoutCode(model.FirstName, model.LastName, model.EncryptedMail);
                        return new ServiceResult<object>().Ok(result.Data);
                }
            }
            else
            {
                return new ServiceResult<object>().Ok((int)SignStatus.UserNameExists);
            }
            return new ServiceResult<object>().Ok(0);
        }
        #endregion

        public async Task<bool> CheckUserName(string userName)
        {
            var cacheKey = "UserNames";
            string serializedCustomerList;
            var userNames = new List<string>();
            var UserNameList = await distributedCache.GetAsync(cacheKey);
            if (UserNameList != null)
            {
                serializedCustomerList = Encoding.UTF8.GetString(UserNameList);
                userNames = JsonConvert.DeserializeObject<List<string>>(serializedCustomerList);
            }
            else
            {
                userNames = _userRepository.GetQuery().Select(z => z.UserName).ToList();
                serializedCustomerList = JsonConvert.SerializeObject(userNames);
                UserNameList = Encoding.UTF8.GetBytes(serializedCustomerList);
                await distributedCache.SetAsync(cacheKey, UserNameList);
            }
            if (userNames.Contains(userName))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public async Task<IServiceResult<SignStatus>> ValidateConfirmationCode(string phoneNumber, string code)
        {
            var CodeCache = await _cache.GetAsync(phoneNumber);
            if (CodeCache != null)
            {
                if (code.Equals(Encoding.UTF8.GetString(CodeCache)))
                {
                    return new ServiceResult<SignStatus>().Ok(SignStatus.Success);
                }
                else
                {
                    return new ServiceResult<SignStatus>().Ok(SignStatus.WrongCode);
                }
            }
            else
            {
                return new ServiceResult<SignStatus>().Ok(SignStatus.CodeExpire);
            }

        }
        public async Task<IServiceResult<User>> UpdateUser(User user)
        {
            user.UpdateDate = System.DateTime.UtcNow;
            await _userRepository.Update(user);
            return new ServiceResult<User>().Ok(user);
        }
        public async Task<IServiceResult<User>> GetById(int userId)
        {
            var model = _userRepository.GetQuery().FirstOrDefault(z => z.UserId == userId);
            model.Password = null;
            return new ServiceResult<User>().Ok(model);
        }
        public Task<IServiceResult<List<User>>> GetByIds(List<int> ids)
        {
            var model = _userRepository.GetQuery().Where(p => ids.Any(i => i.Equals(p.UserId))).ToList();
            model.ForEach(z => z.Password = null);
            return Task.FromResult(new ServiceResult<List<User>>().Ok(model));
        }
        public async Task<IServiceResult> DeleteUser(int userId)
        {
            var user = (await GetById(userId)).Data;
            await _userRepository.Delete(user);
            return new ServiceResult().Ok();
        }

        //public async Task<IServiceResult> RemoveUser(long userId)
        //{
        //    try
        //    {
        //        string cmd = $"Delete [User].Users where UserId = {userId} "+
        //          $" Delete[User].UserSettings where UserId = {userId} "+
        //           $" Delete Social.ViewHistories where UserId = {userId} "+
        //           $" Delete Social.RoomateDocs Where UserId = {userId} "+
        //           $" Delete Social.PostComments where UserId = {userId} "+
        //           $" Delete Social.PostLikes where UserId = {userId} "+
        //           $" Delete Social.Messages where UserId = {userId} "+
        //           $" Delete Social.Posts where UserId = {userId} "+
        //           $" Delete Social.Reviews where UserId = {userId} "+
        //           $" Delete Social.FollowRequests where FollowerUserId = {userId} and FollowingUserId = {userId} "+
        //           $" Delete Social.Followings where FollowingUserId = {userId} "+
        //           $" Delete Social.Followers where FollowerUserId = {userId} "+
        //           $" Delete Social.AccountSettings where UserId = {userId} "+
        //           $" Delete Social.Activities where UserId = {userId} "+
        //           $" Delete Social.MessageRecipients where UserId = {userId} "+
        //           $" Delete Common.Tickets where UserId = {userId} "+
        //           $" Delete Common.Logs where UserId = {userId} "+
        //           $" Delete Common.WishLists where UserId = {userId} "+
        //           $" Delete Common.PostReports where ReporterUserId =1 and UserId = {userId} "+
        //           $" Delete Service.Roomates where UserId = {userId} "+
        //           $" Delete Service.Ads where UserId = {userId} "+
        //           $" Delete Service.AdsComments where UserId = {userId} "+
        //           $" Delete Service.UserComments where UserId = {userId}";

        //        await _userRepository.DapperSqlQuery(cmd);
        //        var cacheKey = "UserNames";
        //        var userNames = _userRepository.GetQuery().Select(z => z.UserName).ToList();
        //        var serializedCustomerList = JsonConvert.SerializeObject(userNames);
        //        var UserNameList = Encoding.UTF8.GetBytes(serializedCustomerList);
        //        await distributedCache.SetAsync(cacheKey, UserNameList);
        //        return new ServiceResult().Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult().Failure("something went wrong !!!");
        //    }

        //}
        //public async Task<IServiceResult<object>> SendConfirmationCode(string phoneNumber)
        //{
        //    var hasUser = (await _userRepository.GetQuery().Where(u => u.Mobile.Equals(phoneNumber)).AnyAsync()) ? 1 : 0;
        //    if (hasUser == 0)
        //    {
        //        if (await _cache.GetAsync(phoneNumber) is null)
        //        {
        //            var random = new Random();
        //            var confirmCode = new string(Enumerable.Repeat(_confirmationCodeSetting.Chars, _confirmationCodeSetting.CodeLength).Select(s => s[random.Next(_confirmationCodeSetting.Chars.Length)]).ToArray());
        //            //UltraFastSend member = new UltraFastSend()
        //            //{
        //            //    Mobile = Convert.ToInt64(phoneNumber),
        //            //    TemplateId = 66659,
        //            //    ParameterArray = new List<UltraFastParameters>()
        //            //{
        //            //    new UltraFastParameters()
        //            //    {
        //            //        Parameter = "VerificationCode" , ParameterValue = $"{confirmCode}"
        //            //    }
        //            //}.ToArray()
        //            //};
        //            //var result = await _smsSenderService.UltraFastSend(member);
        //            if (result.IsSuccess)
        //            {
        //                byte[] bytes = Encoding.UTF8.GetBytes(confirmCode.ToCharArray());
        //                await _cache.SetAsync(phoneNumber, bytes, new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(4) });
        //                return new ServiceResult<object>().Ok(new { hasUser, message = "A Confirmation code has been sent to you" });
        //            }
        //            return new ServiceResult<object>().Ok(new { hasUser, message = result.ErrorMessage });
        //        }
        //        return new ServiceResult<object>().Ok(new { hasUser, message = "Confirmation code has not expired" });
        //    }
        //    else
        //    {
        //        return new ServiceResult<object>().Ok(new { hasUser, message = "User Exists" });
        //    }
        //}
        public async Task<IServiceResult<object>> SendConfirmationCodeToBoth(string phoneNumber, string email)
        {
            if (email == null)
            {
                email = "";
            }
            var hasUser = (await _userRepository.GetQuery().Where(u => u.Mobile.Equals(phoneNumber)).AnyAsync()) ? 1 : 0;
            if (email != "")
            {
                hasUser = (await _userRepository.GetQuery().Where(u => u.Email.Equals(email)).AnyAsync()) ? 1 : hasUser;
            }
            if (hasUser == 0)
            {
                bool PhoneNumberCheck = false;
                bool EmailCheck = false;
                if (phoneNumber == null)
                {
                    PhoneNumberCheck = true;
                }
                else
                {
                    if (await _cache.GetAsync(phoneNumber) is null)
                    {
                        PhoneNumberCheck = true;
                    }
                    else
                    {
                        PhoneNumberCheck = false;
                    }
                }


                if (PhoneNumberCheck == true && await _cache.GetAsync(email) is null)
                {
                    var random = new Random();
                    var confirmCode = new string(Enumerable.Repeat(_confirmationCodeSetting.Chars, _confirmationCodeSetting.CodeLength).Select(s => s[random.Next(_confirmationCodeSetting.Chars.Length)]).ToArray());

                    if (phoneNumber != null && phoneNumber.Length > 5)
                    {
                        var bulkSendResult = await _sMsService.SendVerificationCode( confirmCode, phoneNumber);

                        if (bulkSendResult.Status == 1)
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(confirmCode.ToCharArray());
                            await _cache.SetAsync(phoneNumber, bytes, new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(4) });
                            //return new ServiceResult<object>().Ok(new { hasUser, message = "A Confirmation code has been sent to you" });
                        }
                    }
                    if (email.Contains("@"))
                    {
                        MimeMessage message = new MimeMessage();

                        MailboxAddress from = new MailboxAddress("Malieh Iran",
                        "herbodfisherbot@gmail.com");
                        message.From.Add(from);
                        MailboxAddress to = new MailboxAddress("User",
                        $"{email}");
                        message.To.Add(to);
                        message.Subject = "Sign";

                        BodyBuilder bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = $"<h1>Hello Dear Subscriber!</h1> <br/> <p>{confirmCode}</p>";
                        message.Body = bodyBuilder.ToMessageBody();

                        SmtpClient client = new SmtpClient();
                        client.Connect("smtp.gmail.com", 465, true);
                        client.Authenticate("herbodfisherbot@gmail.com", "typaxyklzsolcihv");

                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                    }

                    byte[] bytes1 = Encoding.UTF8.GetBytes(confirmCode.ToCharArray());
                    await _cache.SetAsync(email, bytes1, new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(4) });


                    return new ServiceResult<object>().Ok(new { hasUser, message = "A Confirmation code has been sent to you" });
                }
                return new ServiceResult<object>().Ok(new { hasUser, message = "Confirmation code has not expired" });
            }
            else
            {
                return new ServiceResult<object>().Ok(new { hasUser, message = "User Exists" });
            }






            //var hasUser = (await _userRepository.GetQuery().Where(u => u.Mobile.Equals(Email)).AnyAsync()) ? 1 : 0;
            //if (await _cache.GetAsync(Email) is null)
            //{
            //    try
            //    {
            //        var random = new Random();
            //        var confirmCode = new string(Enumerable.Repeat(_confirmationCodeSetting.Chars, _confirmationCodeSetting.CodeLength).Select(s => s[random.Next(_confirmationCodeSetting.Chars.Length)]).ToArray());
            //        MimeMessage message = new MimeMessage();

            //        MailboxAddress from = new MailboxAddress("Malieh Iran",
            //        "herbodfisherbot@gmail.com");
            //        message.From.Add(from);
            //        MailboxAddress to = new MailboxAddress("User",
            //        $"{Email}");
            //        message.To.Add(to);
            //        message.Subject = "Sign";

            //        BodyBuilder bodyBuilder = new BodyBuilder();
            //        bodyBuilder.HtmlBody = $"<h1>Hello Dear Subscriber!</h1> <br/> <p>{confirmCode}</p>";
            //        message.Body = bodyBuilder.ToMessageBody();

            //        SmtpClient client = new SmtpClient();
            //        client.Connect("smtp.gmail.com", 465, true);
            //        client.Authenticate("herbodfisherbot@gmail.com", "typaxyklzsolcihv");

            //        client.Send(message);
            //        client.Disconnect(true);
            //        client.Dispose();
            //        byte[] bytes = Encoding.UTF8.GetBytes(confirmCode.ToCharArray());
            //        await _cache.SetAsync(Email, bytes, new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(4) });
            //        return new ServiceResult<object>().Ok(new { hasUser, message = "A Confirmation code has been sent to your Email" });
            //    }
            //    catch (Exception ex)
            //    {
            //        return new ServiceResult<object>().Ok(new { hasUser, message = "network problem" });
            //    }
            //}
            //return new ServiceResult<object>().Ok(new { hasUser, message = "Confirmation code has not expired" });
        }

        public async Task<IServiceResult<object>> ForgotPassword(AuthTypes type, string PhoneOrEmail)
        {
            string PassWord = "";
            User dbUser = new User();
            if (type == AuthTypes.Email)
            {
                dbUser = _userRepository.GetQuery().FirstOrDefault(z => z.Email == PhoneOrEmail);
            }
            if (type == AuthTypes.Mobile)
            {
                dbUser = _userRepository.GetQuery().FirstOrDefault(z => z.Mobile == PhoneOrEmail);
            }
            if (dbUser == null)
            {
                var Obj = new
                {
                    Status = (int)SignStatus.NoUserFound
                };
                return new ServiceResult<object>().Ok(Obj);
            }
            PassWord = _decryptService.Decrypt(dbUser.Password).Data;

            if (type == AuthTypes.Email)
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Malieh Iran",
                "herbodfisherbot@gmail.com");
                message.From.Add(from);
                MailboxAddress to = new MailboxAddress("User",
                $"{PhoneOrEmail}");
                message.To.Add(to);
                message.Subject = "Forgot Password";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $"your password is {PassWord}";
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("herbodfisherbot@gmail.com", "typaxyklzsolcihv");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            if (type == AuthTypes.Mobile)
            {
                await _sMsService.ForgotPassword(PassWord,dbUser.Mobile);
            }

            var Obj1 = new
            {
                Status = (int)SignStatus.Success
            };
            return new ServiceResult<object>().Ok(Obj1);

        }
        public string creatRandomPassword()
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQSTUVWXYZabcdefghijklmnpqrstuvwxyz!@#$%^&*";
            Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$");
            Random randomNnmber = new Random();
            int len = randomNnmber.Next(8, 15);//get the max(15) and min(8)
            string randomStr = "";

            for (int i = 0; i < len; i++)
            {
                randomStr += chars[randomNnmber.Next(chars.Length)];
            }
            Match match = regex.Match(randomStr);
            if (!match.Success)
            {
                Console.WriteLine($"{randomStr}");
                return creatRandomPassword();
            }
            else
            {
                Console.WriteLine($"{randomStr}");
                return randomStr;
            }
        }

        public async Task<IServiceResult<object>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                string PassWord = "";
                User dbUser = new User();

                if (changePasswordDto.Type == AuthTypes.Mobile)
                {
                    dbUser = _userRepository.GetQuery().FirstOrDefault(z => z.Mobile == changePasswordDto.Phone);
                }
                if (dbUser == null)
                {
                    var Obj = new
                    {
                        Status = (int)SignStatus.NoUserFound
                    };
                    return new ServiceResult<object>().Ok(Obj);
                }
                PassWord = _decryptService.Decrypt(dbUser.Password).Data;

                if (PassWord != changePasswordDto.OldPassword)
                {
                    var Obj = new
                    {
                        Message = "WrongPass",
                        Status = (int)SignStatus.WrongUserPass
                    };
                    return new ServiceResult<object>().Ok(Obj);
                }
                else
                {

                    #region update password

                    Regex regex = new Regex(@"^.{4,12}$");
                    Match match = regex.Match(changePasswordDto.NewPassword);
                    if (match.Success)
                    {
                        dbUser.Password = _encryptService.Encrypt(changePasswordDto.NewPassword).Data;
                        dbUser.UpdateDate = System.DateTime.UtcNow;
                        await _userRepository.Update(dbUser);
                    }
                    else
                    {
                        return new ServiceResult<object>().Ok((int)SignStatus.PassFormatNotMatched);
                    }

                    #endregion

                }

                var Obj1 = new
                {
                    Status = (int)SignStatus.Success
                };
                return new ServiceResult<object>().Ok(Obj1);
            }
            catch (Exception ex)
            {
                return new ServiceResult<object>().Failure(ex.Message);
            }

        }
    }
}
