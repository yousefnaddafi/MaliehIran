using IPE.SmsIrClient.Models.Results;
using MaliehIran.Services.SMSServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaliehIran.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly ISMSService _sMSService;
        public SMSController(ISMSService sMSService)
        {
            _sMSService = sMSService;
        }
        [HttpPost]
        public async Task<SmsIrResult<VerifySendResult>> SendVerificationCode(string code, string mobileNumber)
        {
            return await _sMSService.SendVerificationCode(code, mobileNumber);
        }
        [HttpPost]
        public async Task ForgotPassword(string code, string mobileNumber)
        {
            await _sMSService.ForgotPassword(code,mobileNumber);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task SendReport(long userId)
        {
            await _sMSService.SendReport(userId);
        }
    }
}
