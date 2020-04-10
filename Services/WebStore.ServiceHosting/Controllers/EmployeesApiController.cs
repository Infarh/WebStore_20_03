using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.Models;
using WebStore.Interfaces.Services;

namespace WebStore.ServiceHosting.Controllers
{
    /// <summary>Контроллер управления сотрудниками</summary>
    //[Route("api/[controller]")]
    [Route(WebAPI.Employees)]
    [ApiController]
    public class EmployeesApiController : ControllerBase, IEmployeesData
    {
        private readonly IEmployeesData _EmployeesData;

        public EmployeesApiController(IEmployeesData EmployeesData) => _EmployeesData = EmployeesData;

        /// <summary>Получить всех сотрудников</summary>
        /// <returns>Список сотрудников магазина</returns>
        [HttpGet]
        public IEnumerable<Employee> GetAll() => _EmployeesData.GetAll();

        /// <summary>Получить сотрудника по идентификатору</summary>
        /// <param name="id">Идентификатор запрашиваемого сотрудника</param>
        /// <returns>Сотрудник с указанным идентификатором</returns>
        [HttpGet("{id}")]
        public Employee GetById(int id) => _EmployeesData.GetById(id);

        /// <summary>Добавить сотрудника</summary>
        /// <param name="Employee">Новый сотрудник магазина</param>
        [HttpPost]
        public void Add([FromBody] Employee Employee) => _EmployeesData.Add(Employee);

        /// <summary>Редактирование сотрудника</summary>
        /// <param name="id">Идентификатор редактируемого сотрудника</param>
        /// <param name="Employee">Информация, вносимая в БД о сотруднике</param>
        [HttpPut("{id}")]
        public void Edit(int id, [FromBody] Employee Employee) => _EmployeesData.Edit(id, Employee);

        /// <summary>Удалить сотрудника</summary>
        /// <param name="id">Идентификатор удаляемого сотрудника</param>
        /// <returns>Истина, если сотрудник успешно удалён</returns>
        [HttpDelete("{id}")]
        public bool Delete(int id) => _EmployeesData.Delete(id);

        [NonAction]
        public void SaveChanges() => _EmployeesData.SaveChanges();
    }
}