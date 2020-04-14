using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels.Identity;

namespace WebStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _UserManager;
        private readonly SignInManager<User> _SignInManager;
        private readonly ILogger<AccountController> _Logger;

        public AccountController(
            UserManager<User> UserManager, 
            SignInManager<User> SignInManager,
            ILogger<AccountController> Logger)
        {
            _UserManager = UserManager;
            _SignInManager = SignInManager;
            _Logger = Logger;
        }

        #region Регистрация пользователя в системе

        public IActionResult Register() => View(new RegisterUserViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel Model, [FromServices] IMapper Mapper)
        {
            if (!ModelState.IsValid)
                return View(Model);

            var user = Mapper.Map<User>(Model);

            using (_Logger.BeginScope("Создание нового пользователя {0}", Model.UserName))
            {
                var register_result = await _UserManager.CreateAsync(user, Model.Password);
                if (register_result.Succeeded)
                {
                    _Logger.LogInformation("Пользователь {0} успешно зарегистрирован", Model.UserName);

                    await _UserManager.AddToRoleAsync(user, Role.User);

                    _Logger.LogInformation("Пользователю {0} добавлена роль {1}", 
                        Model.UserName, Role.User);

                    await _SignInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in register_result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                _Logger.LogWarning("Ошибка при создании пользователя {0}:{1}",
                    Model.UserName, string.Join(", ", register_result.Errors.Select(e => e.Description)));
            }

            return View(Model);
        }

        #endregion

        #region Вход пользователя в систему

        public IActionResult Login(string ReturnUrl) => View(new LoginViewModel { ReturnUrl = ReturnUrl });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel Model)
        {
            if (!ModelState.IsValid) return View(Model);

            var login_result = await _SignInManager.PasswordSignInAsync(
                Model.UserName,
                Model.Password,
                Model.RememberMe,
                false);

            if (login_result.Succeeded)
            {
                _Logger.LogInformation("Пользователь {0} успешно вошёл в систему", Model.UserName);

                if (Url.IsLocalUrl(Model.ReturnUrl))
                    return Redirect(Model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Неверное имя пользователя, или пароль");
            _Logger.LogWarning("Ошибка ввода учётных данных пользователем {0}", Model.UserName);

            return View(Model);
        } 

        #endregion

        public async Task<IActionResult> Logout()
        {
            var user_name = User.Identity.Name;
            await _SignInManager.SignOutAsync();

            _Logger.LogInformation("Пользователь {0} вышел из системы", user_name);

            return RedirectToAction("Index", "Home");
        }
    }
}