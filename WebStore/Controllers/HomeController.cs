using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
            //return View("OtherViewName");
        }

        public IActionResult SomeAction()
        {
            return View();
        }
    }
}