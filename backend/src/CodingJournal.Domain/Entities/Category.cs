namespace CodingJournal.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}