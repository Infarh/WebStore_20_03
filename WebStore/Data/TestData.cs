using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Models;

namespace WebStore.Data
{
    public class TestData
    {
        public static List<Employee> Employees { get; } = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                SurName = "Иванов",
                FirstName = "Иван",
                Patronymic = "Иванович",
                Age = 39
            },
            new Employee
            {
                Id = 2,
                SurName = "Петров",
                FirstName = "Пётр",
                Patronymic = "Петрович",
                Age = 18
            },
            new Employee
            {
                Id = 3,
                SurName = "Сидоров",
                FirstName = "Сидор",
                Patronymic = "Сидорович",
                Age = 27
            },
        };
    }
}
