using AzilEdu.Api.Data;
using Microsoft.EntityFrameworkCore;
using AzilEdu.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AzilEduDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AzilEduDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Animals.AnyAsync())
    {
        db.Animals.AddRange(
            new Animal
            {
                Name = "Luna",
                Species = "Pas",
                Breed = "Labrador",
                Gender = "Ženka",
                Age = 3,
                ArrivalDate = new DateTime(2025, 10, 12),
                AnimalStatusId = 1,
                ImageUrl = "/images/animals/luna.webp",
                Description = "Mirna i druželjubiva kujica koja voli šetnje."
            },
            new Animal
            {
                Name = "Maza",
                Species = "Mačka",
                Breed = "Domaća kratkodlaka",
                Gender = "Ženka",
                Age = 2,
                ArrivalDate = new DateTime(2025, 11, 5),
                AnimalStatusId = 3,
                ImageUrl = "/images/animals/maza.webp",
                Description = "Zaigrana mačka naviknuta na boravak u zatvorenom prostoru."
            },
            new Animal
            {
                Name = "Rex",
                Species = "Pas",
                Breed = "Njemački ovčar",
                Gender = "Mužjak",
                Age = 5,
                ArrivalDate = new DateTime(2026, 1, 20),
                AnimalStatusId = 1,
                ImageUrl = "/images/animals/rex.webp",
                Description = "Aktivan pas koji traži iskusnijeg vlasnika."
            },
            new Animal
            {
                Name = "Nala",
                Species = "Mačka",
                Breed = "Maine Coon mješanac",
                Gender = "Ženka",
                Age = null,
                ArrivalDate = new DateTime(2026, 2, 3),
                AnimalStatusId = 1,
                ImageUrl = "/images/animals/nala.webp",
                Description = "Mlada mačka pronađena bez poznate povijesti."
            },
            new Animal
            {
                Name = "Tobi",
                Species = "Pas",
                Breed = "Mješanac",
                Gender = "Mužjak",
                Age = 1,
                ArrivalDate = null,
                AnimalStatusId = 2,
                ImageUrl = "/images/animals/tobi.webp",
                Description = "Vesel pas kojem datum dolaska još nije potvrđen."
            },
            new Animal
            {
                Name = "Bruno",
                Species = "Pas",
                Breed = "Bigl",
                Gender = "Mužjak",
                Age = 4,
                ArrivalDate = new DateTime(2025, 9, 18),
                AnimalStatusId = 3,
                ImageUrl = "/images/animals/bruno.webp",
                Description = "Udomljen pas koji ostaje u evidenciji azila."
            }
        );
    }

    if (!await db.HousingUnits.AnyAsync())
    {
        db.HousingUnits.AddRange(
            new HousingUnit
            {
                Id = 1,
                Name = "Boks 1",
                UnitType = "Boks za pse",
                Capacity = 4,
                Occupied = 2,
                LastCleanedAt = new DateTime(2026, 6, 15),
                IsActive = true,
                ImageUrl = "/images/housing-units/box-1.webp",
                Note = "Boks za pse srednje veličine, ima slobodnih mjesta."
            },
            new HousingUnit
            {
                Id = 2,
                Name = "Boks 2",
                UnitType = "Boks za pse",
                Capacity = 3,
                Occupied = 3,
                LastCleanedAt = new DateTime(2026, 6, 16),
                IsActive = true,
                ImageUrl = "/images/housing-units/box-2.webp",
                Note = "Boks za pse, trenutačno popunjen do kraja."
            },
            new HousingUnit
            {
                Id = 3,
                Name = "Mačja soba",
                UnitType = "Soba",
                Capacity = 6,
                Occupied = 4,
                LastCleanedAt = new DateTime(2026, 6, 14),
                IsActive = true,
                ImageUrl = "/images/housing-units/cat-room.webp",
                Note = "Zatvorena soba za mačke s prostorom za penjanje."
            },
            new HousingUnit
            {
                Id = 4,
                Name = "Karantena",
                UnitType = "Karantena",
                Capacity = 2,
                Occupied = 1,
                LastCleanedAt = null,
                IsActive = true,
                ImageUrl = "/images/housing-units/quarantine.webp",
                Note = "Prostor za novopristigle životinje, datum čišćenja još nije unesen."
            },
            new HousingUnit
            {
                Id = 5,
                Name = "Vanjski boks",
                UnitType = "Vanjski prostor",
                Capacity = 5,
                Occupied = 1,
                LastCleanedAt = new DateTime(2026, 6, 10),
                IsActive = true,
                ImageUrl = "/images/housing-units/yard-unit.webp",
                Note = "Ograđeni vanjski prostor s puno slobodnih mjesta."
            },
            new HousingUnit
            {
                Id = 6,
                Name = "Stara soba",
                UnitType = "Soba",
                Capacity = 4,
                Occupied = 0,
                LastCleanedAt = new DateTime(2026, 5, 2),
                IsActive = false,
                ImageUrl = "/images/housing-units/inactive-unit.webp",
                Note = "Jedinica privremeno izvan upotrebe zbog obnove."
            }
        );
    }

    if (!await db.Volunteers.AnyAsync())
    {
        db.Volunteers.AddRange(
            new Volunteer
            {
                FirstName = "Ana",
                LastName = "Horvat",
                Email = "ana.horvat@example.com",
                Phone = "091 111 2222",
                Skills = "Šetnja pasa, socijalizacija",
                AvailableFrom = new DateTime(2026, 7, 1),
                Notes = "Dostupna vikendom.",
                VolunteerStatusId = 2
            },
            new Volunteer
            {
                FirstName = "Marko",
                LastName = "Kovač",
                Email = "marko.kovac@example.com",
                Phone = "092 333 4444",
                Skills = "Prijevoz, pomoć kod veterinara",
                AvailableFrom = new DateTime(2026, 7, 10),
                Notes = "Ima vlastiti automobil.",
                VolunteerStatusId = 1
            }
        );
    }

    if (!await db.Donors.AnyAsync())
    {
        db.Donors.AddRange(
            new Donor
            {
                FirstName = "Ivana",
                LastName = "Babić",
                Email = "ivana.babic@example.com",
                Phone = "095 555 1212",
                Address = "Ulica donatora 1",
                City = "Osijek",
                Notes = "Donira hranu jednom mjesečno.",
                CreatedAt = new DateTime(2026, 6, 15),
                DonorTypeId = 1,
                DonorStatusId = 2
            },
            new Donor
            {
                OrganizationName = "Pet Plus d.o.o.",
                Email = "kontakt@petplus.example.com",
                Phone = "031 555 000",
                Address = "Industrijska 12",
                City = "Osijek",
                Notes = "Potencijalni donator opreme.",
                CreatedAt = new DateTime(2026, 6, 20),
                DonorTypeId = 2,
                DonorStatusId = 1
            }
        );
    }

    if (!await db.Employees.AnyAsync())
    {
        db.Employees.AddRange(
            new Employee
            {
                FirstName = "Petra",
                LastName = "Novak",
                Email = "petra.novak@aziledu.example.com",
                Phone = "031 100 200",
                EmployeeNumber = "EMP-001",
                HireDate = new DateTime(2025, 3, 1),
                Notes = "Koordinira raspored volontera.",
                EmployeePositionId = 3,
                EmployeeStatusId = 1
            },
            new Employee
            {
                FirstName = "Ivan",
                LastName = "Marić",
                Email = "ivan.maric@aziledu.example.com",
                Phone = "031 100 201",
                EmployeeNumber = "EMP-002",
                HireDate = new DateTime(2024, 9, 10),
                Notes = "Zadužen za svakodnevnu brigu o životinjama.",
                EmployeePositionId = 1,
                EmployeeStatusId = 1
            }
        );
    }

        await db.SaveChangesAsync();
        await AppUserSeeder.SeedAsync(db);
    }

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Posluzuje datoteke iz wwwroot, npr. prenesene medije na /uploads/animals/...
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
