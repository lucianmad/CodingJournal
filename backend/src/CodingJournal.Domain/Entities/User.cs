using Microsoft.AspNetCore.Identity;

namespace CodingJournal.Domain.Entities;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}