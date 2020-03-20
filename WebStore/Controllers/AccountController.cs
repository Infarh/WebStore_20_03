using Microsoft.AspNetCore.Mvc;
using WebStore.ViewModels.Identity;

namespace WebStore.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register() => View(new RegisterUserViewModel());

        public IActionResult Login() => View(new LoginViewModel());
    }
}