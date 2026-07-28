using AzilEdu.Api.Data;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzilEdu.Shared.DTOs;

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
    public async Task<ActionResult<List<HousingUnitDto>>> GetHousingUnits()
    {
        var housingUnit = await _context.HousingUnits
            .OrderBy(h => h.Name)
            .Select(h => new HousingUnitDto
            {
                Id = h.Id,
                Name = h.Name,
                UnitType = h.UnitType,
                Occupied = h.Occupied,
                Capacity = h.Capacity,
                IsActive = h.IsActive,
                LastCleanedAt = h.LastCleanedAt,
                ImageUrl = h.ImageUrl,
                Note = h.Note
            }).ToListAsync();

        return Ok(housingUnit);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HousingUnitDto>> GetHousingUnitById(int id)
    {
        var housingUnit = await _context.HousingUnits.FindAsync(id);
        if (housingUnit is null)
        {
            return NotFound();
        }
        var dto = new HousingUnitDto
        {
            Id = housingUnit.Id,
            Name = housingUnit.Name,
            UnitType = housingUnit.UnitType,
            Occupied = housingUnit.Occupied,
            Capacity = housingUnit.Capacity,
            IsActive = housingUnit.IsActive,
            LastCleanedAt = housingUnit.LastCleanedAt,
            ImageUrl = housingUnit.ImageUrl,
            Note = housingUnit.Note
        };
        return Ok(dto);
    }


    [HttpPost]
    public async Task<ActionResult<HousingUnitDto>> CreateHousingUnit(SaveHousingUnitDto dto)
    {
        var housingUnit = new HousingUnit
        {
            Name = dto.Name,
            UnitType = dto.UnitType,
            Capacity = dto.Capacity,
            Occupied = dto.Occupied,
            IsActive = dto.IsActive,
            LastCleanedAt = dto.LastCleanedAt,
            ImageUrl = dto.ImageUrl,
            Note = dto.Note
        };

        _context.HousingUnits.Add(housingUnit);
        await _context.SaveChangesAsync();

        var result = new HousingUnitDto
        {
            Id = housingUnit.Id,
            Name = housingUnit.Name,
            UnitType = housingUnit.UnitType,
            Capacity = housingUnit.Capacity,
            Occupied = housingUnit.Occupied,
            IsActive = housingUnit.IsActive,
            LastCleanedAt = housingUnit.LastCleanedAt,
            ImageUrl = housingUnit.ImageUrl,
            Note = housingUnit.Note
        };

        return CreatedAtAction(nameof(GetHousingUnitById), new { id = housingUnit.Id }, result);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHousingUnit(int id, SaveHousingUnitDto dto)
    {
        var housingUnit = await _context.HousingUnits.FindAsync(id);

        if (housingUnit is null)
            return NotFound();

        housingUnit.Name = dto.Name;
        housingUnit.UnitType = dto.UnitType;
        housingUnit.Capacity = dto.Capacity;
        housingUnit.Occupied = dto.Occupied;
        housingUnit.IsActive = dto.IsActive;
        housingUnit.LastCleanedAt = dto.LastCleanedAt;
        housingUnit.ImageUrl = dto.ImageUrl;
        housingUnit.Note = dto.Note;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHousingUnit(int id)
    {
        var housingUnit = await _context.HousingUnits.FindAsync(id);

        if (housingUnit is null)
            return NotFound();

        _context.HousingUnits.Remove(housingUnit);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}