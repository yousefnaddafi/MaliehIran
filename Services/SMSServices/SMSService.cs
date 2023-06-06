using IPE.SmsIrClient.Models.Requests;
using IPE.SmsIrClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography.X509Certificates;
using IPE.SmsIrClient.Models.Results;
using MaliehIran.Services.UserServices;
using MaliehIran.Infrastructure;
using MaliehIran.Models;
using System.Linq;

namespace MaliehIran.Services.SMSServices
{
    public class SMSService : ISMSService
    {
        private readonly IConfiguration _configuration;
        private readonly IProjectEFRepository<User> _userRepository;
        
        public SMSService(IConfiguration configuration, IProjectEFRepository<User> userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }
        //public async Task VerifySend(string code,string mobileNumber)
        //{
        //    var ApiKey = _configuration.GetSection("SmsSetting").GetSection("UserApiKey").Value;
        //    var SenderNumber = _configuration.GetSection("SmsSetting").GetSection("LineNumber").Value;
        //    SmsIr smsIr = new SmsIr(ApiKey);

        //    var bulkSendResult = await smsIr.BulkSendAsync(Convert.ToInt64(SenderNumber), code, new string[] { mobileNumber });

        //    var verificationSendResult = await smsIr.VerifySendAsync(mobileNumber, 100000, new VerifySendParameter[] { new VerifySendParameter("Code", code) });
        //}

        public async Task<SmsIrResult<VerifySendResult>> SendVerificationCode(string code,string mobileNumber)
        {
            var ApiKey = _configuration.GetSection("SmsSetting").GetSection("UserApiKey").Value;
            SmsIr smsIr = new SmsIr(ApiKey);

            //var SenderNumber = _configuration.GetSection("SmsSetting").GetSection("LineNumber").Value;
            //var bulkSendResult = await smsIr.BulkSendAsync(Convert.ToInt64(SenderNumber), code, new string[] { mobileNumber });

            var verificationSendResult = await smsIr.VerifySendAsync(mobileNumber, 793543, new VerifySendParameter[] { new VerifySendParameter("Code", code) });
            return verificationSendResult;
        }
        public async Task ForgotPassword(string code, string mobileNumber)
        {
            var ApiKey = _configuration.GetSection("SmsSetting").GetSection("UserApiKey").Value;
            SmsIr smsIr = new SmsIr(ApiKey);

            //var SenderNumber = _configuration.GetSection("SmsSetting").GetSection("LineNumber").Value;
            //var bulkSendResult = await smsIr.BulkSendAsync(Convert.ToInt64(SenderNumber), code, new string[] { mobileNumber });

            var verificationSendResult = await smsIr.VerifySendAsync(mobileNumber, 850488, new VerifySendParameter[] { new VerifySendParameter("Password", code) });
        }
        public async Task SendReport(long userId)
        {
            var ApiKey = _configuration.GetSection("SmsSetting").GetSection("UserApiKey").Value;
            SmsIr smsIr = new SmsIr(ApiKey);
            var dbUser = _userRepository.GetQuery().FirstOrDefault(z => z.UserId == userId);
            var mobileNumber = "";
            var UserName = "";
            if(dbUser != null)
            {
                mobileNumber = dbUser.Mobile;
                UserName = dbUser.UserName;
                var verificationSendResult = await smsIr.VerifySendAsync(mobileNumber, 643098, new VerifySendParameter[] { new VerifySendParameter("User", UserName) });
            }
        }
    }
}
