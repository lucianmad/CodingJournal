using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
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
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result<DocumentDto>.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;
        
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