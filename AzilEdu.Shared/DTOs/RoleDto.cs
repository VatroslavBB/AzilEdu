namespace AzilEdu.Shared.DTOs;

// Uloga koju API salje App projektu. Id sluzi za spremanje veze AppUserRole.
public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
