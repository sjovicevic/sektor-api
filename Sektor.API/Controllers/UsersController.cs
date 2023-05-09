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
    [HttpGet(Name = "GetUsers")]
    [HttpHead]
    public async Task<IActionResult> Get([FromQuery] UsersResourceParameters  usersResourceParameters)
    {
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