using IPE.SmsIrClient.Models.Requests;
using IPE.SmsIrClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography.X509Certificates;
using IPE.SmsIrClient.Models.Results;

namespace MaliehIran.Services.SMSServices
{
    public class SMSService : ISMSService
    {
        private readonly IConfiguration _configuration;
        
        public SMSService(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public async Task SendReport(string UserName, string mobileNumber)
        {
            var ApiKey = _configuration.GetSection("SmsSetting").GetSection("UserApiKey").Value;
            SmsIr smsIr = new SmsIr(ApiKey);

            //var SenderNumber = _configuration.GetSection("SmsSetting").GetSection("LineNumber").Value;
            //var bulkSendResult = await smsIr.BulkSendAsync(Convert.ToInt64(SenderNumber), code, new string[] { mobileNumber });

            var verificationSendResult = await smsIr.VerifySendAsync(mobileNumber, 643098, new VerifySendParameter[] { new VerifySendParameter("User", UserName) });
        }
    }
}
