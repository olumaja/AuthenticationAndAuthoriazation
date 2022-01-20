using AuthenticationAndAuthoriazation.Models;

namespace AuthenticationAndAuthoriazation.Services
{
    public class EmployeeRepository
    {
        public List<Employee> Employees { get; set; } = new List<Employee>
        {
            new Employee{FirstName = "Paul", LastName = "James"},
            new Employee{FirstName = "Kola", LastName = "Bode"}
        };

        public List<Employee> GetAllEmployees()
        {
            return Employees;
        }
    }
}
