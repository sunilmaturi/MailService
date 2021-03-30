using MailKit.Net.Smtp;
using MailKit.Security;
using MailService.Models;
using MailService.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace MailService.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private ISmtpClient _smtpClient;
        public MailService(IOptions<MailSettings> mailSettings, ISmtpClient smtpClient)
        {
            _mailSettings = mailSettings.Value;
            _smtpClient = smtpClient;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            //using var smtp = new SmtpClient();
            _smtpClient.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            _smtpClient.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await _smtpClient.SendAsync(email);
            _smtpClient.Disconnect(true);
        }
    }
}
