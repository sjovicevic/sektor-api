using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Microsoft.AspNetCore.Authorization;
using Sektor.API.src.Core.Validators;
using Sektor.API.src.Core.Errors;
using Sektor.API.src.Core.Extensions;
using Sektor.API.src.ResourceParameters;
using Sektor.API.src.Helpers;
using System.Text.Json;

namespace Sektor.API.Controllers;


[Route("[controller]")]
[ApiController]
[Authorize]
public class MembershipsController : ControllerBase
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMembershipTypeRepository _membershipTypeRepository;
    private readonly IMapper _mapper;
    private readonly CreateMembershipValidator _createMembershipValidator;
    private readonly ILogger<MembershipsController> _logger;

    public MembershipsController(
        IMembershipRepository membershipRepository,
        IUserRepository userRepository,
        IMembershipTypeRepository membershipTypeRepository,
        IMapper mapper,
        CreateMembershipValidator createMembershipValidator,
        ILogger<MembershipsController> logger)
    {
        _membershipRepository = membershipRepository;
        _userRepository = userRepository;
        _membershipTypeRepository = membershipTypeRepository;
        _mapper = mapper;
        _createMembershipValidator = createMembershipValidator;
        _logger = logger;
    }

    // GET: api/<MembershipsController>
    [HttpGet(Name = "GetMemberships")]
    public async Task<IActionResult> Get([FromQuery] MembershipsResourceParameters membershipsResourceParameters)
    {
        _logger.LogInformation("Received request to get memberships. [{time}]", DateTime.Now);
        _logger.LogInformation("Fetching memberships...");
        var memberships = await _membershipRepository.GetAllMembershipsAsync(membershipsResourceParameters);

        var previousPageLink = memberships.HasPrevious ?
            CreateMembershipsResourceUri(membershipsResourceParameters, ResourceUriType.PreviousPage) : null;

        var nextPageLink = memberships.HasNext ?
            CreateMembershipsResourceUri(membershipsResourceParameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new 
        {
            totalCount = memberships.TotalCount,
            pageSize = memberships.PageSize,
            currentPage = memberships.CurrentPage,
            totalPages = memberships.TotalPages,
            previousPageLink = previousPageLink,
            nextPageLink = nextPageLink
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        _logger.LogInformation("Finished with fetching. [{time}]", DateTime.Now);

        return Ok(_mapper.Map<List<MembershipDto>>(memberships));
    }

    private string? CreateMembershipsResourceUri(
        MembershipsResourceParameters membershipsResourceParameters,
        ResourceUriType type)
    {
        switch(type) 
        {
            case ResourceUriType.PreviousPage:
                return Url.Link("GetMemberships",
                new {
                    pageNumber = membershipsResourceParameters.PageNumber - 1,
                    pageSize = membershipsResourceParameters.PageSize,
                    searchQuery = membershipsResourceParameters.SearchQuery
                });
            case ResourceUriType.NextPage:
                return Url.Link("GetMemberships",
                new {
                    pageNumber = membershipsResourceParameters.PageNumber + 1,
                    pageSize = membershipsResourceParameters.PageSize,
                    searchQuery = membershipsResourceParameters.SearchQuery
                });
            default:
                return Url.Link("GetMemberships",
                new {
                    pageNumber = membershipsResourceParameters.PageNumber,
                    pageSize = membershipsResourceParameters.PageSize,
                    searchQuery = membershipsResourceParameters.SearchQuery
                });
        }
    }

    // GET api/<MembershipsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        _logger.LogInformation("Received request to get membership with ID {id}. [{time}]", id, DateTime.Now);
        _logger.LogInformation("Fetching membership info...");
        var membership = await _membershipRepository.GetMembershipByIdAsync(id);

        if (membership == null)
        {
            _logger.LogInformation("Membership with ID {id} not found. [{time}]", id, DateTime.Now);
            return NotFound();
        }

        _logger.LogInformation("Finished. [{time}].", DateTime.Now);

        return Ok(_mapper.Map<MembershipDto>(membership));
    }
    

    // POST api/<MembershipsController>
    [HttpPost("{userId}/{membershipTypeId}")]
    public async Task<IActionResult> Post(int userId, int membershipTypeId, [FromBody] MembershipCreationDto dto)
    {
        _logger.LogInformation("Received request to create membership UserId: {userId} MembershipTypeId: {membershipTypeId} [{time}]", userId, membershipTypeId,  DateTime.Now);
        _logger.LogInformation("Creating membership...");
        var user = await _userRepository.GetUserByIdAsync(userId);
        var membershipType = await _membershipTypeRepository.GetMembershipTypeByIdAsync(membershipTypeId);

        if(user == null || membershipType == null)
        {
            _logger.LogInformation("User or membership type not found. [{time}]", DateTime.Now);
            return NotFound();
        }

        var result = _createMembershipValidator.Validate(dto);

        if (!result.IsValid)
        {
            _logger.LogError("Membership creation failed. [{time}]", DateTime.Now);
            return result.AsClientErrors();
        }

        _membershipRepository.AddNewMembership(dto, user, membershipType);

        await _membershipRepository.SaveChangesAsync();

        _logger.LogInformation("Finished. [{time}].", DateTime.Now);

        return StatusCode(201);
    }

    // PUT api/<MembershipsController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] MembershipCreationDto dto)
    {
        _logger.LogInformation("Received request to update membership with ID {id} [{time}]", id, DateTime.Now);
        _logger.LogInformation("Updating membership...");

        var membership = await _membershipRepository.GetMembershipByIdAsync(id);

        if(membership == null)
        {
            _logger.LogInformation("Membership with ID {id} not found. [{time}]", id, DateTime.Now);
            return NotFound();
        }

        _membershipRepository.UpdateMembership(membership, dto);

        try
        {
            await _membershipRepository.SaveChangesAsync();
            _logger.LogInformation("Finished. [{time}].", DateTime.Now);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Membership update failed. [{time}]", DateTime.Now);
            return BadRequest(ex.Message);
        }
    }

    // DELETE api/<MembershipsController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Received request to delete  membership  with ID {id} [{time}]", id, DateTime.Now);
        _logger.LogInformation("Deleting membership...");

        var membership = await _membershipRepository.GetMembershipByIdAsync(id);
        if (membership == null)
        {
            _logger.LogInformation("Membership with ID {id} not found. [{time}]", id, DateTime.Now);
            return NotFound();
        }

        _membershipRepository.DeleteMembership(membership);

        try
        {
            await _membershipRepository.SaveChangesAsync();
            _logger.LogInformation("Finished. [{time}].", DateTime.Now);
            return NoContent();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Membership deletion failed. [{time}]", DateTime.Now);
            return BadRequest(ex.Message);
        }
    }
}