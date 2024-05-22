using BaseProject.Models;

namespace WebApp.Observer.Observer
{
    public class UserObserverSendMail : IUserObserver
    {
        private readonly IServiceProvider _serviceProvider;
        public UserObserverSendMail(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void UserCreated(AppUser appUser)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<UserObserverSendMail>>();

            /*
             * Mail gönderme işlemleri
             */

            logger.LogInformation($"mail send to {appUser.UserName}");
        }
    }
}
