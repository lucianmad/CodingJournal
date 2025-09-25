namespace CodingJournal.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}