using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DonationsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<DonationDto>>> GetDonations(
        [FromQuery] int? typeId,
        [FromQuery] int? statusId,
        [FromQuery] int? donorId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var query = _context.Donations
            .Include(donation => donation.Donor)
            .Include(donation => donation.DonationType)
            .Include(donation => donation.DonationStatus)
            .AsQueryable();

        // Kasnije će donator vidjeti samo svoje donacije.

        if (typeId.HasValue)
        {
            query = query.Where(donation => donation.DonationTypeId == typeId.Value);
        }

        if (statusId.HasValue)
        {
            query = query.Where(donation => donation.DonationStatusId == statusId.Value);
        }

        if (donorId.HasValue)
        {
            query = query.Where(donation => donation.DonorId == donorId.Value);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(donation => donation.DonationDate >= dateFrom.Value.Date);
        }

        if (dateTo.HasValue)
        {
            // dodan jos jedan dan zato sto zelimo ubrajati cijeli taj dan u filter
            var endExclusive = dateTo.Value.Date.AddDays(1);
            query = query.Where(donation => donation.DonationDate < endExclusive);
        }

        var donations = await query
            .OrderByDescending(donation => donation.DonationDate)
            .ThenBy(donation => donation.Id)
            .ToListAsync();

        var result = donations
            .Select(ToDto)
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonationDto>> GetDonationById(int id)
    {
        var donation = await _context.Donations
            .Include(item => item.Donor)
            .Include(item => item.DonationType)
            .Include(item => item.DonationStatus)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (donation is null)
        {
            return NotFound();
        }

        return Ok(ToDto(donation));
    }

    [HttpPost]
    public async Task<ActionResult<DonationDto>> CreateDonation(SaveDonationDto request)
    {
        var donation = new Donation
        {
            DonorId = request.DonorId,
            DonationTypeId = request.DonationTypeId,
            DonationStatusId = request.DonationStatusId,
            DonationDate = request.DonationDate,
            Amount = request.Amount,
            ItemName = request.ItemName,
            Quantity = request.Quantity,
            EstimatedValue = request.EstimatedValue,
            Notes = request.Notes
        };

        _context.Donations.Add(donation);
        await _context.SaveChangesAsync();

        var createdDonation = await _context.Donations
            .Include(item => item.Donor)
            .Include(item => item.DonationType)
            .Include(item => item.DonationStatus)
            .FirstAsync(item => item.Id == donation.Id);

        return CreatedAtAction(
            nameof(GetDonationById),
            new { id = donation.Id },
            ToDto(createdDonation));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDonation(int id, SaveDonationDto request)
    {
        var donation = await _context.Donations.FindAsync(id);

        if (donation is null)
        {
            return NotFound();
        }

        donation.DonorId = request.DonorId;
        donation.DonationTypeId = request.DonationTypeId;
        donation.DonationStatusId = request.DonationStatusId;
        donation.DonationDate = request.DonationDate;
        donation.Amount = request.Amount;
        donation.ItemName = request.ItemName;
        donation.Quantity = request.Quantity;
        donation.EstimatedValue = request.EstimatedValue;
        donation.Notes = request.Notes;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDonation(int id)
    {
        var donation = await _context.Donations.FindAsync(id);

        if (donation is null)
        {
            return NotFound();
        }

        _context.Donations.Remove(donation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static DonationDto ToDto(Donation donation)
    {
        return new DonationDto
        {
            Id = donation.Id,
            DonorId = donation.DonorId,
            DonorName = donation.Donor != null
                ? (!string.IsNullOrWhiteSpace(donation.Donor.OrganizationName)
                    ? donation.Donor.OrganizationName
                    : donation.Donor.FirstName + " " + donation.Donor.LastName)
                : string.Empty,
            DonationTypeId = donation.DonationTypeId,
            Type = donation.DonationType != null ? donation.DonationType.Name : string.Empty,
            DonationStatusId = donation.DonationStatusId,
            Status = donation.DonationStatus != null ? donation.DonationStatus.Name : string.Empty,
            DonationDate = donation.DonationDate,
            Amount = donation.Amount,
            ItemName = donation.ItemName,
            Quantity = donation.Quantity,
            EstimatedValue = donation.EstimatedValue,
            Notes = donation.Notes
        };
    }
}
