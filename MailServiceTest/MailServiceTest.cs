using MailKit.Net.Smtp;
using MailService.Models;
using MailService.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;
using ms = MailService.Services;

namespace MailServiceTest
{
    public class MailServiceTest
    {
        private Mock<ISmtpClient> _smtpClient;

        public MailServiceTest()
        {
            _smtpClient = new Mock<ISmtpClient>();
        }
        [Fact]
        public void Mail_Success_Test()
        {            
            var mailSettings = Options.Create(new MailSettings() { Mail = "test@example.com", DisplayName = "Test", Host = "Host", Password = "password", Port = 587});
            
            var service = new ms.MailService(mailSettings, _smtpClient.Object);
            var response = service.SendEmailAsync(new MailRequest() { ToEmail = "test1@example.com", Body = "Test Body", Subject = "Test Subject"});
            Assert.Equal(TaskStatus.RanToCompletion, response.Status);
        }
    }
}
