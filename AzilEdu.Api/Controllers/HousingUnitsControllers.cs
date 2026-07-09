using AzilEdu.Api.Data;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HousingUnitsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public HousingUnitsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<HousingUnit>>> GetHousingUnits()
    {
        var housingUnit = await _context.HousingUnits
            .OrderBy(housingUnit => housingUnit.Name)
            .ToListAsync();

        return Ok(housingUnit);
    }
}