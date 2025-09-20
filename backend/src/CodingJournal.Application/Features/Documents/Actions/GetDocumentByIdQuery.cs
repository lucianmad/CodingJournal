using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Features.Documents.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Documents.Actions;

public record GetDocumentByIdQuery(int Id) : IRequest<Result<DocumentDto>>;

public class GetDocumentByIdQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetDocumentByIdQuery, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        
        var document = await context.Documents
            .Include(d => d.User)
            .Include(d => d.Category)
            .Where(d => d.UserId == userId)
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