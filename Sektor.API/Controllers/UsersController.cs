using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Context;
using Sektor.API.src.Contracts;
using Sektor.API.src.Core.Errors;
using Sektor.API.src.Core.Extensions;
using Sektor.API.src.Core.Validators;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly CreateUserValidator _createUserValidator;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(
        IUserRepository userRepository,
        IMapper mapper, 
        CreateUserValidator createUserValidator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _createUserValidator = createUserValidator;
    }

    // GET api/users
    [HttpGet]
    public async Task<IActionResult> Get(string? name)
    {
        var users = await _userRepository.GetAllUsersAsync(name);
        return Ok(_mapper.Map<List<UserDto>>(users));
    }

    // GET api/users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if(user == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<UserDto>(user));
    }

    // POST api/users
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserCreationDto userCreationDto)
    {
        var result = _createUserValidator.Validate(userCreationDto);

        if (!result.IsValid)
        {
            return result.AsClientErrors();
        }

        try
        {
            _userRepository.AddNewUser(userCreationDto);
            await _userRepository.SaveChangesAsync();
            return StatusCode(201);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    // PUT api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, 
        [FromBody] UserCreationDto userCreationDto)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }
        
        var result = _createUserValidator.Validate(userCreationDto);

        if (!result.IsValid)
        {
            return result.AsClientErrors();
        }
        _userRepository.UpdateUser(user, userCreationDto); 
        try
        {
            await _userRepository.SaveChangesAsync();
            return StatusCode(204);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    // DELETE api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if(user == null)
        {
            return NotFound();
        }

        _userRepository.DeleteUser(user);
        try
        {
            await _userRepository.SaveChangesAsync();
            return StatusCode(204);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}