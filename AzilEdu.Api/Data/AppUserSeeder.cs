using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Data;

public static class AppUserSeeder
{
    // Uloge iz AzilEduDbContext seeda.
    private const int RoleUser = 1;
    private const int RoleAdmin = 2;
    private const int RoleEmployee = 3;
    private const int RoleVolunteer = 4;
    private const int RoleDonor = 5;

    public static async Task SeedAsync(AzilEduDbContext db)
    {
        // Radi po korisniku, ne po praznoj tablici, pa se novi demo korisnik
        // doda i na bazi koja vec ima ranije seedane korisnike.
        await EnsureUserAsync(db, "user@aziledu.local", "Demo korisnik", "User123!",
            RoleUser);

        await EnsureUserAsync(db, "admin@aziledu.local", "AzilEdu Admin", "Admin123!",
            RoleUser, RoleAdmin);

        await EnsureUserAsync(db, "employee@aziledu.local", "Djelatnik azila", "Employee123!",
            RoleUser, RoleEmployee);

        await EnsureUserAsync(db, "volunteer@aziledu.local", "Demo volonter", "Volunteer123!",
            RoleUser, RoleVolunteer);

        await EnsureUserAsync(db, "donor@aziledu.local", "Demo donator", "Donor123!",
            RoleUser, RoleDonor);

        // Ako je poslovni zapis dodan naknadno, tek se sada moze povezati.
        await LinkDemoUsersAsync(db);
    }

    private static async Task EnsureUserAsync(
        AzilEduDbContext db,
        string email,
        string displayName,
        string password,
        params int[] roleIds)
    {
        if (await db.AppUsers.AnyAsync(item => item.Email == email))
            return;

        var hasher = new PasswordHasher<AppUser>();

        var user = new AppUser
        {
            Email = email,
            DisplayName = displayName
        };

        user.PasswordHash = hasher.HashPassword(user, password);

        db.AppUsers.Add(user);
        await db.SaveChangesAsync();

        foreach (var roleId in roleIds)
            db.AppUserRoles.Add(new AppUserRole { AppUserId = user.Id, AppRoleId = roleId });

        await db.SaveChangesAsync();
    }

    private static async Task LinkDemoUsersAsync(AzilEduDbContext db)
    {
        var changed = false;

        var employee = await db.AppUsers
            .FirstOrDefaultAsync(item => item.Email == "employee@aziledu.local");

        if (employee is not null && employee.EmployeeId is null)
        {
            // Cast na int? je bitan: bez njega prazna tablica vraca 0,
            // a EmployeeId = 0 rusi strani kljuc umjesto da ostane null.
            employee.EmployeeId = await db.Employees
                .Select(item => (int?)item.Id)
                .FirstOrDefaultAsync();

            changed |= employee.EmployeeId is not null;
        }

        var volunteer = await db.AppUsers
            .FirstOrDefaultAsync(item => item.Email == "volunteer@aziledu.local");

        if (volunteer is not null && volunteer.VolunteerId is null)
        {
            volunteer.VolunteerId = await db.Volunteers
                .Select(item => (int?)item.Id)
                .FirstOrDefaultAsync();

            changed |= volunteer.VolunteerId is not null;
        }

        var donor = await db.AppUsers
            .FirstOrDefaultAsync(item => item.Email == "donor@aziledu.local");

        if (donor is not null && donor.DonorId is null)
        {
            donor.DonorId = await db.Donors
                .Select(item => (int?)item.Id)
                .FirstOrDefaultAsync();

            changed |= donor.DonorId is not null;
        }

        if (changed)
            await db.SaveChangesAsync();
    }
}
