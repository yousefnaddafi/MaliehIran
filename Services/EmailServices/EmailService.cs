using MaliehIran.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfirmationCodeSetting _confirmationCodeSetting;
        public EmailService(IConfirmationCodeSetting confirmationCodeSetting)
        {
            _confirmationCodeSetting = confirmationCodeSetting;
        }
        public async Task<object> SendMail(string subject, string content, string emailTo)
        {
            try
            {
                MimeMessage message = new MimeMessage();
                MailboxAddress from = new MailboxAddress("Project", "info@example.com");
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("User", $"{emailTo}");
                message.To.Add(to);

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = content;
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect("11.111.111.111", 587, false);
                client.Authenticate("info@example.com", "PASSWORD");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
                return new { message = "Done" };
            }
            catch (Exception ex)
            {
                return new { message = "network problem" };
            }
        }
    }
}
