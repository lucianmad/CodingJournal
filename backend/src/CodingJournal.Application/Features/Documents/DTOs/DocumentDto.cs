namespace CodingJournal.Application.Features.Documents.DTOs;

public record DocumentDto(int Id, string Title, string Content, DateTime CreatedAt, DateTime UpdatedAt, string? Username, string? CategoryName);