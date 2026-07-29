using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public DashboardController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var today = DateTime.Today;

        var moneyAmounts = await _context.Donations
            .Where(donation => donation.DonationTypeId == 1 && donation.Amount != null)
            .Select(donation => donation.Amount!.Value)
            .ToListAsync();

        var materialValues = await _context.Donations
            .Where(donation => donation.DonationTypeId != 1 && donation.EstimatedValue != null)
            .Select(donation => donation.EstimatedValue!.Value)
            .ToListAsync();

        var summary = new DashboardSummaryDto
        {
            AnimalsCount = await _context.Animals.CountAsync(),
            AvailableAnimalsCount = await _context.Animals.CountAsync(animal => animal.AnimalStatusId == 1),
            ActiveVolunteersCount = await _context.Volunteers.CountAsync(volunteer => volunteer.VolunteerStatusId == 2),
            OpenVolunteerTasksCount = await _context.VolunteerTasks.CountAsync(task => task.VolunteerTaskStatusId == 1),
            ActiveDonorsCount = await _context.Donors.CountAsync(donor => donor.DonorStatusId == 2),
            EmployeesCount = await _context.Employees.CountAsync(),

            DonationsCount = await _context.Donations.CountAsync(),

            PendingDonationsCount = await _context.Donations.CountAsync(donation => donation.DonationStatusId == 1),

            MoneyDonationsTotal = moneyAmounts.Sum(),
            EstimatedMaterialDonationsTotal = materialValues.Sum(),

            OverdueVolunteerTasksCount = await _context.VolunteerTasks.CountAsync(task =>
                task.DueDate != null
                && task.DueDate < today
                && task.CompletedAt == null)
        };

        return Ok(summary);
    }

    [HttpGet("recent-donations")]
    public async Task<ActionResult<List<RecentDonationDto>>> GetRecentDonations()
    {
        var donations = await _context.Donations
            .Include(donation => donation.Donor)
            .Include(donation => donation.DonationType)
            .OrderByDescending(donation => donation.DonationDate)
            .ThenByDescending(donation => donation.Id)
            .Take(5)
            .ToListAsync();

        var result = donations
            .Select(donation => new RecentDonationDto
            {
                Id = donation.Id,
                DonorName = donation.Donor != null
                    ? (!string.IsNullOrWhiteSpace(donation.Donor.OrganizationName)
                        ? donation.Donor.OrganizationName
                        : donation.Donor.FirstName + " " + donation.Donor.LastName)
                    : string.Empty,
                DonationType = donation.DonationType != null ? donation.DonationType.Name : string.Empty,
                DonationDate = donation.DonationDate,
                Amount = donation.Amount,
                ItemName = donation.ItemName
            })
            .ToList();

        return Ok(result);
    }
}
