using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Sektor.API.Controllers;

[Route("authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

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


    public class AuthenticationRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate(
        AuthenticationRequestBody authenticationRequestBody)
    {
        var user = ValidateUserCredentials(
            authenticationRequestBody.UserName,
            authenticationRequestBody.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>();
        claimsForToken.Add(new Claim("sub", user.EmployeeId.ToString()));
        claimsForToken.Add(new Claim("name", user.EmployeeUserName));
        claimsForToken.Add(new Claim("admin", user.Admin.ToString()));

        
        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }

    private Employee ValidateUserCredentials(string? userName, string? password)
    {
        var listOfEmployees = new List<Employee>
        {
            new Employee(1, "jovanapalavestra", true, "Sektor442020"),
            new Employee(2, "stefanjovicevic", false, "stefan")
        };

        var employee = listOfEmployees.FirstOrDefault(
            x => x.EmployeeUserName == userName && x.Password == password);
        
        return employee;
    }
}