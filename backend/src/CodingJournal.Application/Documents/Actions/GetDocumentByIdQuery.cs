using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Documents.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Documents.Actions;

public record GetDocumentByIdQuery(string UserId, int Id) : IRequest<Result<DocumentDto>>;

public class GetDocumentByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetDocumentByIdQuery, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await context.Documents
            .Include(d => d.User)
            .Include(d => d.Category)
            .Where(d => d.UserId == request.UserId)
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