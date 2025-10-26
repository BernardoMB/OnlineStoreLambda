using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OnlineStoreLambda
{
    public class MailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly IConfiguration _config;

        private const string Host = "smtp.gmail.com";
        private const int Port = 587;
        private readonly string _username;
        private readonly string _password;

        public MailService(ILogger<MailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _username = _config["PERSONAL_EMAIL"];
            _password = _config["GoogleAppPassword"];
        }

        public void SendEmail(string fromAddress, string fromDisplay, string to, string cc, string subject, string plainTextBody, string? htmlBody = null, string? attachmentPath = null)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            using var smtp = new SmtpClient
            {
                Host = Host,
                Port = Port,
                EnableSsl = true,
                Credentials = new NetworkCredential(_username, _password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(fromAddress, fromDisplay),
                Subject = subject,
                Body = htmlBody ?? plainTextBody,
                IsBodyHtml = htmlBody != null
            };

            mail.To.Add(to);
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(cc);
            }

            if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
            {
                mail.Attachments.Add(new Attachment(attachmentPath));
            }

            try
            {
                smtp.Send(mail);
                _logger.LogInformation("Email sent to {Recipient}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}. Inner: {Inner}", to, ex.InnerException?.Message);
                _logger.LogError(ex, "Failed to send email to {Recipient}", to);
            }
        }
    }
}
