using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;
using Sektor.API.src.Core.Validators;
using Sektor.API.src.Core.Errors;
using Sektor.API.src.Core.Extensions;

namespace Sektor.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MembershipTypesController : ControllerBase
{
    private readonly IMembershipTypeRepository _membershipTypeRepository;
    private readonly IMapper _mapper;

    private readonly CreateMembershipTypeValidator _createMembershipTypeValidator;

    public MembershipTypesController(
        IMembershipTypeRepository membershipTypeRepository,
        IMapper mapper,
        CreateMembershipTypeValidator createMembershipTypeValidator
    )
    {
        _membershipTypeRepository = membershipTypeRepository;
        _mapper = mapper;
        _createMembershipTypeValidator = createMembershipTypeValidator;
    }
    // GET: api/<MembershipTypesController>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MembershipType>>> Get()
    {
        var membershipTypes = await _membershipTypeRepository.GetAllMembershipTypesAsync();
        return Ok(_mapper.Map<List<MembershipTypeDto>>(membershipTypes));    
    }

    // GET api/<MembershipTypesController>/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<MembershipType>> Get(int id)
    {
        var membershipType = await _membershipTypeRepository.GetMembershipTypeByIdAsync(id);

        if(membershipType == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<MembershipTypeDto>(membershipType));
    }

    // POST api/<MembershipTypesController>
    [HttpPost]
    [Authorize(Policy = "ManagerPolicy")]
    public async Task<IActionResult> Post(MembershipTypeCreationDto dto)
    {
        var result = await _createMembershipTypeValidator.ValidateAsync(dto);

        if(!result.IsValid)
        {
            return result.AsClientErrors();
        }
        _membershipTypeRepository.AddNewMembershipType(dto);
        try
        {
            await _membershipTypeRepository.SaveChangesAsync();
            return StatusCode(201);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    // PUT api/<MembershipTypesController>/5
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerPolicy")]
    public async Task<ActionResult> Put(int id, [FromBody] MembershipTypeCreationDto dto)
    {
        var membershipType = await _membershipTypeRepository.GetMembershipTypeByIdAsync(id);

        if(membershipType == null)
        {
            return NotFound();
        }

        _membershipTypeRepository.UpdateMembershipType(membershipType, dto);

        try
        {
            await _membershipTypeRepository.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    // DELETE api/<MembershipTypesController>/5
    [HttpDelete("{id}")]
    [Authorize(Policy = "ManagerPolicy")]
    public async Task<ActionResult> Delete(int id)
    {
        var membershipType = await _membershipTypeRepository.GetMembershipTypeByIdAsync(id);

        if(membershipType == null)
        {
            return NotFound();
        }

        bool membershipExists = _membershipTypeRepository.CheckIfMembershipExists(membershipType);

        if(membershipExists) 
        {
            return BadRequest(new ClientError {
                PropertyName = "MembershipTypeId",
                ErrorMessage = "Membership type is in use and cannot be deleted."
            });
        }

        _membershipTypeRepository.DeleteMembershipType(membershipType);

        try
        {
            await _membershipTypeRepository.SaveChangesAsync();
            return StatusCode(204);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }

    }
}