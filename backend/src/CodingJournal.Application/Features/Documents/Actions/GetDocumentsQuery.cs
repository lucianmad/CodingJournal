using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using CodingJournal.Application.Features.Documents.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Documents.Actions;

public record GetDocumentsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    int? CategoryId = null) : IRequest<Result<PagedList<DocumentDto>>>;

public class GetDocumentsQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetDocumentsQuery, Result<PagedList<DocumentDto>>>
{
    public async Task<Result<PagedList<DocumentDto>>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result<PagedList<DocumentDto>>.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;
        
        var query = context.Documents
            .Include(d => d.User)
            .Include(d => d.Category)
            .Where(d => d.UserId == userId);
        
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(d => d.Title.Contains(request.SearchTerm));
        }
        
        if (request.CategoryId.HasValue)
        {
            query = query.Where(d => d.CategoryId == request.CategoryId);
        }
        
        var totalCount = await query.CountAsync(cancellationToken);

        var documents = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DocumentDto
                (
                    d.Id,
                    d.Title,
                    d.Content,
                    d.CreatedAt,
                    d.UpdatedAt,
                    d.User.Email,
                    d.Category != null ? d.Category.Name : null
                )
            )
            .ToListAsync(cancellationToken);
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        
        var pagedResult = new PagedList<DocumentDto>(documents, request.Page, request.PageSize, totalCount, totalPages);
        
        return Result<PagedList<DocumentDto>>.Success(pagedResult);
    }
}