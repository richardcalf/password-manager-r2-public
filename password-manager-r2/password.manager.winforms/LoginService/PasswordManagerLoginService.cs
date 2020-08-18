using password.model;
using password.uibroker;

namespace password.manager.winforms
{
    public class PasswordManagerLoginService : IPasswordManagerLoginService
    {
        IUIBroker broker;

        public PasswordManagerLoginService(IUIBroker broker)
        {
            this.broker = broker;
        }

        public bool Login(Login login)
        {
            login.Site = "admin.admin";
            return broker.LoginService.Login(login);
        }

        public bool Register(Login login)
        {
            login.Site = "admin.admin";
            return broker.LoginService.Register(login);
        }
    }
}
