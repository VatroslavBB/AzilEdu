using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonorsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DonorsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<DonorDto>>> GetDonors()
    {
        var donors = await _context.Donors
            .Include(d => d.DonorType)
            .Include(d => d.DonorStatus)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.OrganizationName)
            .Select(d => new DonorDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                OrganizationName = d.OrganizationName,
                DisplayName = d.OrganizationName != string.Empty
                    ? d.OrganizationName
                    : d.FirstName + " " + d.LastName,
                Email = d.Email,
                Phone = d.Phone,
                Address = d.Address,
                City = d.City,
                Notes = d.Notes,
                CreatedAt = d.CreatedAt,
                DonorTypeId = d.DonorTypeId,
                Type = d.DonorType != null ? d.DonorType.Name : string.Empty,
                DonorStatusId = d.DonorStatusId,
                Status = d.DonorStatus != null ? d.DonorStatus.Name : string.Empty
            })
            .ToListAsync();

        return Ok(donors);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<LookupDto>>> GetDonorsLookup()
    {
        var donors = await _context.Donors
            .OrderBy(donor => donor.OrganizationName)
            .ThenBy(donor => donor.LastName)
            .ThenBy(donor => donor.FirstName)
            .Select(donor => new LookupDto
            {
                Id = donor.Id,
                Name = donor.OrganizationName != string.Empty
                    ? donor.OrganizationName
                    : donor.FirstName + " " + donor.LastName
            })
            .ToListAsync();

        return Ok(donors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonorDto>> GetDonorById(int id)
    {
        var donor = await _context.Donors
            .Include(d => d.DonorType)
            .Include(d => d.DonorStatus)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (donor is null)
            return NotFound();

        var dto = new DonorDto
        {
            Id = donor.Id,
            FirstName = donor.FirstName,
            LastName = donor.LastName,
            OrganizationName = donor.OrganizationName,
            DisplayName = GetDisplayName(donor),
            Email = donor.Email,
            Phone = donor.Phone,
            Address = donor.Address,
            City = donor.City,
            Notes = donor.Notes,
            CreatedAt = donor.CreatedAt,
            DonorTypeId = donor.DonorTypeId,
            Type = donor.DonorType != null ? donor.DonorType.Name : string.Empty,
            DonorStatusId = donor.DonorStatusId,
            Status = donor.DonorStatus != null ? donor.DonorStatus.Name : string.Empty
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DonorDto>> CreateDonor(SaveDonorDto dto)
    {
        var donor = new Donor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            OrganizationName = dto.OrganizationName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Notes = dto.Notes,
            CreatedAt = dto.CreatedAt ?? DateTime.Today,
            DonorTypeId = dto.DonorTypeId,
            DonorStatusId = dto.DonorStatusId
        };

        _context.Donors.Add(donor);
        await _context.SaveChangesAsync();

        var savedDonor = await _context.Donors
            .Include(d => d.DonorType)
            .Include(d => d.DonorStatus)
            .FirstOrDefaultAsync(d => d.Id == donor.Id);

        if (savedDonor is null)
            return NotFound();

        var result = new DonorDto
        {
            Id = savedDonor.Id,
            FirstName = savedDonor.FirstName,
            LastName = savedDonor.LastName,
            OrganizationName = savedDonor.OrganizationName,
            DisplayName = GetDisplayName(savedDonor),
            Email = savedDonor.Email,
            Phone = savedDonor.Phone,
            Address = savedDonor.Address,
            City = savedDonor.City,
            Notes = savedDonor.Notes,
            CreatedAt = savedDonor.CreatedAt,
            DonorTypeId = savedDonor.DonorTypeId,
            Type = savedDonor.DonorType != null ? savedDonor.DonorType.Name : string.Empty,
            DonorStatusId = savedDonor.DonorStatusId,
            Status = savedDonor.DonorStatus != null ? savedDonor.DonorStatus.Name : string.Empty
        };

        return CreatedAtAction(nameof(GetDonorById), new { id = donor.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDonor(int id, SaveDonorDto dto)
    {
        var donor = await _context.Donors.FindAsync(id);

        if (donor is null)
            return NotFound();

        donor.FirstName = dto.FirstName;
        donor.LastName = dto.LastName;
        donor.OrganizationName = dto.OrganizationName;
        donor.Email = dto.Email;
        donor.Phone = dto.Phone;
        donor.Address = dto.Address;
        donor.City = dto.City;
        donor.Notes = dto.Notes;
        donor.CreatedAt = dto.CreatedAt;
        donor.DonorTypeId = dto.DonorTypeId;
        donor.DonorStatusId = dto.DonorStatusId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDonor(int id)
    {
        var donor = await _context.Donors.FindAsync(id);

        if (donor is null)
            return NotFound();

        _context.Donors.Remove(donor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string GetDisplayName(Donor donor)
    {
        return !string.IsNullOrWhiteSpace(donor.OrganizationName)
            ? donor.OrganizationName
            : $"{donor.FirstName} {donor.LastName}";
    }
}
