using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly AzilEduDbContext _context;

    public AnimalsController(AzilEduDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnimalDto>>> GetAnimals()
    {
        var animals = await _context.Animals
            .Include(a => a.AnimalStatus)
            .OrderBy(a => a.Name)
            .Select(a => new AnimalDto
            {
                Id = a.Id,
                Name = a.Name,
                Species = a.Species,
                Breed = a.Breed,
                Gender = a.Gender,
                Age = a.Age,
                ArrivalDate = a.ArrivalDate,
                AnimalStatusId = a.AnimalStatusId,
                Status = a.AnimalStatus != null ? a.AnimalStatus.Name : string.Empty,
                // Naslovna slika iz galerije ima prednost, ImageUrl ostaje rezervna.
                ImageUrl = a.Media
                    .Where(media =>
                        media.IsCover &&
                        media.MediaType == AnimalMediaType.Image)
                    .OrderBy(media => media.SortOrder)
                    .Select(media => "/uploads/animals/" + media.StoredFileName)
                    .FirstOrDefault() ?? a.ImageUrl,
                Description = a.Description
            })
            .ToListAsync();

        foreach (var animal in animals)
            animal.ImageUrl = ToPublicImageUrl(animal.ImageUrl);

        return Ok(animals);
    }


    [HttpGet("lookup")]
    public async Task<ActionResult<List<LookupDto>>> GetAnimalsLookup()
    {
        var animals = await _context.Animals
            .OrderBy(animal => animal.Name)
            .Select(animal => new LookupDto
            {
                Id = animal.Id,
                Name = animal.Name
            })
            .ToListAsync();

        return Ok(animals);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnimalDto>> GetAnimalById(int id)
    {
        var animal = await _context.Animals
            .Include(a => a.AnimalStatus)
            .Include(a => a.Media)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal is null)
            return NotFound();

        var dto = new AnimalDto
        {
            Id = animal.Id,
            Name = animal.Name,
            Species = animal.Species,
            Breed = animal.Breed,
            Gender = animal.Gender,
            Age = animal.Age,
            ArrivalDate = animal.ArrivalDate,
            AnimalStatusId = animal.AnimalStatusId,
            Status = animal.AnimalStatus != null ? animal.AnimalStatus.Name : string.Empty,
            ImageUrl = ToPublicImageUrl(GetCoverImagePath(animal)),
            Description = animal.Description
        };

        return Ok(dto);
    }


    [HttpPost]
    public async Task<ActionResult<AnimalDto>> CreateAnimal(SaveAnimalDto dto)
    {
        var animal = new Animal
        {
            Name = dto.Name,
            Species = dto.Species,
            Breed = dto.Breed,
            Gender = dto.Gender,
            Age = dto.Age,
            ArrivalDate = dto.ArrivalDate,
            AnimalStatusId = dto.AnimalStatusId,
            ImageUrl = dto.ImageUrl,
            Description = dto.Description
        };

        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();

        // Ponovno dohvaćamo zapis s Include kako bismo dobili i naziv statusa.
        var savedAnimal = await _context.Animals
            .Include(a => a.AnimalStatus)
            .FirstOrDefaultAsync(a => a.Id == animal.Id);

        if (savedAnimal is null)
            return NotFound();

        var result = new AnimalDto
        {
            Id = savedAnimal.Id,
            Name = savedAnimal.Name,
            Species = savedAnimal.Species,
            Breed = savedAnimal.Breed,
            Gender = savedAnimal.Gender,
            Age = savedAnimal.Age,
            ArrivalDate = savedAnimal.ArrivalDate,
            AnimalStatusId = savedAnimal.AnimalStatusId,
            Status = savedAnimal.AnimalStatus != null ? savedAnimal.AnimalStatus.Name : string.Empty,
            ImageUrl = savedAnimal.ImageUrl,
            Description = savedAnimal.Description
        };

        return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, result);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAnimal(int id, SaveAnimalDto dto)
    {
        var animal = await _context.Animals.FindAsync(id);

        if (animal is null)
            return NotFound();

        animal.Name = dto.Name;
        animal.Species = dto.Species;
        animal.Breed = dto.Breed;
        animal.Gender = dto.Gender;
        animal.Age = dto.Age;
        animal.ArrivalDate = dto.ArrivalDate;
        animal.AnimalStatusId = dto.AnimalStatusId;
        animal.ImageUrl = dto.ImageUrl;
        animal.Description = dto.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
        var animal = await _context.Animals.FindAsync(id);

        if (animal is null)
            return NotFound();

        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string GetCoverImagePath(Animal animal)
    {
        var cover = animal.Media
            .Where(media =>
                media.IsCover &&
                media.MediaType == AnimalMediaType.Image)
            .OrderBy(media => media.SortOrder)
            .FirstOrDefault();

        return cover is null
            ? animal.ImageUrl
            : $"/uploads/animals/{cover.StoredFileName}";
    }

    // Prenesene datoteke poslužuje API na svom portu, pa /uploads/... mora
    // postati puna adresa. Slike iz App projekta (/images/...) ostaju kakve jesu.
    private string ToPublicImageUrl(string imageUrl)
    {
        if (!imageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return imageUrl;

        return $"{Request.Scheme}://{Request.Host}{imageUrl}";
    }
}
