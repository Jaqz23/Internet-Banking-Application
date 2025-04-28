using IB.Core.Application.Dtos.Email;
using IB.Core.Domain.Settings;

namespace IB.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        public MailSettings MailSettings { get; }
        Task SendAsync(EmailRequest request);
    }
}
