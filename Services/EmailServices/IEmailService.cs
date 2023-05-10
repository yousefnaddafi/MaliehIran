using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.EmailServices
{
    public interface IEmailService
    {
        Task<object> SendMail(string subject, string content, string emailTo);
    }
}
