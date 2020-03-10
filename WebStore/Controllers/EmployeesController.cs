using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebStore.Data;
using WebStore.Infrastructure.Interfaces;
using WebStore.Models;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    //[Route("users")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesData _EmployeesData;

        public EmployeesController(IEmployeesData EmployeesData) => _EmployeesData = EmployeesData;

        //[Route("employees")]
        public IActionResult Index() => View(_EmployeesData.GetAll().Select(e => new EmployeeViewModel
        {
            Id = e.Id,
            Name = e.FirstName,
            SecondName = e.SurName,
            Patronymic = e.Patronymic,
            Age = e.Age
        }));

        //[Route("employee/{Id}")]
        public IActionResult Details(int Id)
        {
            var employee = _EmployeesData.GetById(Id);
            if (employee is null)
                return NotFound();

            return View(new EmployeeViewModel
            {
                Id = employee.Id,
                Name = employee.FirstName,
                SecondName = employee.SurName,
                Patronymic = employee.Patronymic,
                Age = employee.Age
            });
        }

        public IActionResult Create()
        {
            return View(new EmployeeViewModel());
        }

        [HttpPost]
        public IActionResult Create(EmployeeViewModel Employee)
        {
            if(Employee is null)
                throw new ArgumentNullException(nameof(Employee));

            if (!ModelState.IsValid)
                return View(Employee);

            _EmployeesData.Add(new Employee
            {
                FirstName = Employee.Name,
                SurName = Employee.SecondName,
                Patronymic = Employee.Patronymic,
                Age = Employee.Age
            });
            _EmployeesData.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? Id)
        {
            if (Id is null) return View(new EmployeeViewModel());

            if (Id < 0)
                return BadRequest();

            var employee = _EmployeesData.GetById((int) Id);
            if (employee is null)
                return NotFound();

            return View(new EmployeeViewModel
            {
                Id = employee.Id,
                Name = employee.FirstName,
                SecondName = employee.SurName,
                Patronymic = employee.Patronymic,
                Age = employee.Age
            });
        }

        [HttpPost]
        public IActionResult Edit(EmployeeViewModel Employee)
        {
            if(Employee is null)
                throw new ArgumentNullException(nameof(Employee));

            if (!ModelState.IsValid)
                return View(Employee);

            var id = Employee.Id;
            if(id == 0)
                _EmployeesData.Add(new Employee
                {
                    FirstName = Employee.Name,
                    SurName = Employee.SecondName,
                    Patronymic = Employee.Patronymic,
                    Age = Employee.Age
                });
            else
                _EmployeesData.Edit(id, new Employee
                {
                    FirstName = Employee.Name,
                    SurName = Employee.SecondName,
                    Patronymic = Employee.Patronymic,
                    Age = Employee.Age
                });

            _EmployeesData.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var employee = _EmployeesData.GetById(id);
            if (employee is null)
                return NotFound();

            return View(new EmployeeViewModel
            {
                Id = employee.Id,
                Name = employee.FirstName,
                SecondName = employee.SurName,
                Patronymic = employee.Patronymic,
                Age = employee.Age
            });
        }

        public IActionResult DeleteConfirmed(int id)
        {
            _EmployeesData.Delete(id);
            _EmployeesData.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}