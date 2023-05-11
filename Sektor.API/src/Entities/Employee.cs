namespace Sektor.API.src.Entities;

public class Employee {
        public int EmployeeId { get; set; }
        public string EmployeeUserName { get; set; }
        public bool Admin { get; init; }
        public string Password { get; init; }

        public Employee(
            int employeeId,
            string employeeUserName,
            bool admin,
            string password
        )
        {
            EmployeeId = employeeId;
            EmployeeUserName = employeeUserName;
            Admin = admin;
            Password = password;
        }
    }