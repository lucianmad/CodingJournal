using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Documents.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Documents.Actions;

public record GetDocumentQuery(int Id) : IRequest<Result<DocumentDto>>;

public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, Result<DocumentDto>>
{
    private readonly IApplicationDbContext _context;
    
    public GetDocumentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<DocumentDto>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents
            .Include(d => d.User)
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        
        if (document == null)
        {
            return Result<DocumentDto>.Failure("Document not found.");
        }
        
        return Result<DocumentDto>.Success(
            new DocumentDto(
                document.Id, 
                document.Title, 
                document.Content, 
                document.CreatedAt, 
                document.UpdatedAt, 
                document.User.Email, 
                document.Category?.Name
                )
        );
    }
}