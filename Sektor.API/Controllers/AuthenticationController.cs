using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using Sektor.API.src.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Cors;

namespace Sektor.API.Controllers;

[Route("auth")]
[ApiController]
[EnableCors]
public class AuthenticationController : ControllerBase
{
    //comment
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public class AuthenticationRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public AuthenticationController(
        IConfiguration configuration,
        IMapper mapper)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpPost]
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
            DateTime.Now.AddHours(24),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        Response.Cookies.Append("Authorization", tokenToReturn);
        return Ok(_mapper.Map<EmployeeDto>(user));
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