namespace CodingJournal.Application.Features.Documents.DTOs;

public record UpdateDocumentRequest(string Title, string Content, int? CategoryId);