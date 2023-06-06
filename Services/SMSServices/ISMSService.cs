using IPE.SmsIrClient.Models.Results;
using System.Threading.Tasks;

namespace MaliehIran.Services.SMSServices
{
    public interface ISMSService
    {
        Task<SmsIrResult<VerifySendResult>> SendVerificationCode(string code, string mobileNumber);
        Task ForgotPassword(string code, string mobileNumber);
        Task SendReport(long userId);
    }
}
