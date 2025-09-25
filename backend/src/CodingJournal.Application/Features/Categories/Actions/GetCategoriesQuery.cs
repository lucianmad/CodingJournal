using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using CodingJournal.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Categories.Actions;

public record GetCategoriesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IRequest<Result<PagedList<CategoryDto>>>;

public class GetCategoriesQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor) 
    : IRequestHandler<GetCategoriesQuery, Result<PagedList<CategoryDto>>>
{
    public async Task<Result<PagedList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result<PagedList<CategoryDto>>.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;
        
        var query = context.Categories
            .Where(c => c.UserId == userId);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var categories = await query.OrderBy(c => c.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(cancellationToken);
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        
        var pagedResult = new PagedList<CategoryDto>(categories, request.Page, request.PageSize, totalCount, totalPages);
        
        return Result<PagedList<CategoryDto>>.Success(pagedResult);
    }
}