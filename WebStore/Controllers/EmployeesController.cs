using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebStore.Data;

namespace WebStore.Controllers
{
    //[Route("users")]
    public class EmployeesController : Controller
    {
        //[Route("employees")]
        public IActionResult Index() => View(TestData.Employees);

        //[Route("employee/{Id}")]
        public IActionResult Details(int Id)
        {
            var employee = TestData.Employees.FirstOrDefault(e => e.Id == Id);
            if (employee is null)
                return NotFound();
            return View(employee);
        }
    }
}