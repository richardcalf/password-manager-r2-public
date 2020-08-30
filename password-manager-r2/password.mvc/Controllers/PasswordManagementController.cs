using password.mvc.Models;
using password.uibroker;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using password.mvc.Extensions;

namespace password.mvc.Controllers
{
    public class PasswordManagementController : Controller
    {

        private IUIBroker broker;

        public PasswordManagementController(IUIBroker broker)
        {
            this.broker = broker;
        }

        // GET: PasswordManagement
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ListPasswords()
        {
            var logins =
                broker.Repo.GetLogins()
                .Select(async l => new LoginViewModel
                {
                    Site = l.Site,
                    UserName = l.UserName,
                    Password = await broker.DecryptAsync(l.Password)
                }).ToList();

            return View(await Task.WhenAll(logins));
        }

        [HttpGet]
        public async Task<ActionResult> ViewLogin(string site)
        {
            var login = broker.Repo.GetLogin(site);
            if (login != null)
            {
                var model = new LoginViewModel
                {
                    Site = login.Site,
                    UserName = login.UserName,
                    Password = await broker.DecryptAsync(login.Password)
                };

                return View(model);
            }
            TempData["Message"] = "That login was not found";
            return RedirectToAction("ListPasswords");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string site)
        {
            var login = broker.Repo.GetLogin(site);

            if (login != null)
            {
                var model = new LoginViewModel
                {
                    Site = login.Site,
                    UserName = login.UserName,
                    Password = await broker.DecryptAsync(login.Password)
                };
                return View(model);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LoginViewModel login)
        {
            if(ModelState.IsValid)
            {
                var model = new model.Login
                {
                    UserName = login.UserName,
                    Site = login.Site,
                    Password = await broker.EncryptAsync(login.Password)
                };
                broker.Repo.Save(model);
                TempData["Message"] = $"{model.Site} Updated";
            }
            
            return RedirectToAction("ViewLogin", new { site = login.Site });
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string site)
        {
            var login = broker.Repo.GetLogin(site);

            if (login != null)
            {
                var model = new LoginViewModel
                {
                    Site = login.Site,
                    UserName = login.UserName,
                    Password = await broker.DecryptAsync(login.Password)
                };
                return View(model);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string site, FormCollection form)
        {
            broker.Repo.Delete(site);
            TempData["Message"] = $"{site} Deleted";
            return RedirectToAction("ListPasswords");
        }



    }
}