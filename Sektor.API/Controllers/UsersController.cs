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
using Sektor.API.src.ResourceParameters;
using Sektor.API.src.Helpers;
using System.Text.Json;

namespace Sektor.API.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
[EnableCors]
public class UsersController : ControllerBase
{
    private readonly CreateUserValidator _createUserValidator;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository userRepository,
        IMapper mapper, 
        CreateUserValidator createUserValidator,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _createUserValidator = createUserValidator;
        _logger = logger;
    }

    // GET api/users
    [HttpGet(Name = "GetUsers")]
    [HttpHead]
    public async Task<IActionResult> Get([FromQuery] UsersResourceParameters  usersResourceParameters)
    {
        _logger.LogInformation("Received request to get users. [{time}]", DateTime.Now);
        _logger.LogInformation("Fetching users...");

        var users = await _userRepository.GetAllUsersAsync(usersResourceParameters);

        var previousPageLink = users.HasPrevious ?
            CreateUsersResourceUri(usersResourceParameters, ResourceUriType.PreviousPage) : null;
        
        var nextPageLink = users.HasNext ?
            CreateUsersResourceUri(usersResourceParameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = users.TotalCount,
            pageSize = users.PageSize,
            currentPage = users.CurrentPage,
            totalPages = users.TotalPages,
            previousPageLink = previousPageLink,
            nextPageLink = nextPageLink
        };

        Response.Headers.Add("X-Pagination",
        JsonSerializer.Serialize(paginationMetadata));

        _logger.LogInformation("Finished with fetching. [{time}]", DateTime.Now);

        return Ok(_mapper.Map<List<UserDto>>(users));
    }

    private string? CreateUsersResourceUri(
        UsersResourceParameters usersResourceParameters,
        ResourceUriType type)
    {
        switch(type) 
        {
            case ResourceUriType.PreviousPage:
                return Url.Link("GetUsers",
                new {
                    pageNumber = usersResourceParameters.PageNumber - 1,
                    pageSize = usersResourceParameters.PageSize,
                    searchQuery = usersResourceParameters.SearchQuery
                });
            case ResourceUriType.NextPage:
                return Url.Link("GetUsers",
                new {
                    pageNumber = usersResourceParameters.PageNumber + 1,
                    pageSize = usersResourceParameters.PageSize,
                    searchQuery = usersResourceParameters.SearchQuery
                });
            default:
                return Url.Link("GetUsers",
                new {
                    pageNumber = usersResourceParameters.PageNumber,
                    pageSize = usersResourceParameters.PageSize,
                    searchQuery = usersResourceParameters.SearchQuery
                });
        }
    }

    // GET api/users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        _logger.LogInformation("Received request to get user with ID {id}. [{time}]", id, DateTime.Now);
        _logger.LogInformation("Fetching user...");
        var user = await _userRepository.GetUserByIdAsync(id);

        if(user == null)
        {
            _logger.LogInformation("User with ID {id} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Finished [{time}].", DateTime.Now);
        return Ok(_mapper.Map<UserDto>(user));
    }

    // POST api/users
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserCreationDto userCreationDto)
    {
        _logger.LogInformation("Received request to create user [{time}]", DateTime.Now);
        _logger.LogInformation("Creating user...");
        var result = _createUserValidator.Validate(userCreationDto);

        if (!result.IsValid)
        {
            _logger.LogInformation("User creation failed.");
            return result.AsClientErrors();
        }

        try
        {
            _userRepository.AddNewUser(userCreationDto);
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("User created.");
            return StatusCode(201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User creation failed.");
            return StatusCode(500);
        }
    }

    // PUT api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, 
        [FromBody] UserCreationDto userCreationDto)
    {
        _logger.LogInformation("Received request to update  user with ID {id} [{time}]", id, DateTime.Now);
        _logger.LogInformation("Updating user...");
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
        {
            _logger.LogInformation("User with ID {id} not found.", id);
            return NotFound();
        }
        
        var result = _createUserValidator.Validate(userCreationDto);

        if (!result.IsValid)
        {
            _logger.LogInformation("User update failed.");
            return result.AsClientErrors();
        }
        _userRepository.UpdateUser(user, userCreationDto); 
        try
        {
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("User updated.");
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User update failed.");
            return StatusCode(500);
        }
    }

    // DELETE api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Received request to delete  user with ID {id} [{time}]", id, DateTime.Now);
        _logger.LogInformation("Deleting user...");
        var user = await _userRepository.GetUserByIdAsync(id);

        if(user == null)
        {
            _logger.LogInformation("User with ID {id} not found.", id);
            return NotFound();
        }

        _userRepository.DeleteUser(user);
        try
        {
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("User deleted.");
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User deletion failed.");
            return StatusCode(500);
        }
    }
}