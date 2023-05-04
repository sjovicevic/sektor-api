using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Sektor.API.src.Entities;

namespace Sektor.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MembershipTypesController : ControllerBase
{
    private readonly IMembershipTypeRepository _membershipTypeRepository;
    private readonly IMapper _mapper;

    public MembershipTypesController(
        IMembershipTypeRepository membershipTypeRepository,
        IMapper mapper
    )
    {
        _membershipTypeRepository = membershipTypeRepository;
        _mapper = mapper;
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
    public async Task<ActionResult> Post(MembershipTypeCreationDto dto)
    {
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

        _membershipTypeRepository.DeleteMembershipType(membershipType);

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
}