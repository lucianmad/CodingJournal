namespace CodingJournal.Domain.Entities;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}