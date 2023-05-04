using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Contracts;
using Sektor.API.src.Dtos;
using Microsoft.AspNetCore.Authorization;

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

    public MembershipsController(
        IMembershipRepository membershipRepository,
        IUserRepository userRepository,
        IMembershipTypeRepository membershipTypeRepository,
        IMapper mapper)
    {
        _membershipRepository = membershipRepository;
        _userRepository = userRepository;
        _membershipTypeRepository = membershipTypeRepository;
        _mapper = mapper;
    }

    // GET: api/<MembershipsController>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var memberships = await _membershipRepository.GetAllMembershipsAsync();
        return Ok(_mapper.Map<List<MembershipDto>>(memberships));
    }


    // GET api/<MembershipsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var membership = await _membershipRepository.GetMembershipByIdAsync(id);

        if (membership == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<MembershipDto>(membership));
    }
    

    // POST api/<MembershipsController>
    [HttpPost("{userId}/{membershipTypeId}")]
    public async Task<ActionResult> Post(int userId, int membershipTypeId, [FromBody] MembershipCreationDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        var membershipType = await _membershipTypeRepository.GetMembershipTypeByIdAsync(membershipTypeId);

        if(user == null || membershipType == null)
        {
            return NotFound();
        }

        _membershipRepository.AddNewMembership(dto, user, membershipType);

        await _membershipRepository.SaveChangesAsync();
        return StatusCode(201);
    }

    // PUT api/<MembershipsController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] MembershipCreationDto dto)
    {
        var membership = await _membershipRepository.GetMembershipByIdAsync(id);

        if(membership == null)
        {
            return NotFound();
        }

        _membershipRepository.UpdateMembership(membership, dto);

        try
        {
            await _membershipRepository.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE api/<MembershipsController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var membership = await _membershipRepository.GetMembershipByIdAsync(id);
        if (membership == null)
            return NotFound();
        _membershipRepository.DeleteMembership(membership);

        try
        {
            await _membershipRepository.SaveChangesAsync();
            return NoContent();
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}