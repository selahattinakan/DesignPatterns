using MediatR;
using WebApp.Observer.Events;

namespace WebApp.Observer.EventHandlers
{
    public class SendEmailEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ILogger<SendEmailEventHandler> _logger;

        public SendEmailEventHandler(ILogger<SendEmailEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            /*
             * Mail gönderme işlemleri
             */

            _logger.LogInformation($"mail send to {notification.AppUser.UserName}");
            return Task.CompletedTask;
        }

    }
}
