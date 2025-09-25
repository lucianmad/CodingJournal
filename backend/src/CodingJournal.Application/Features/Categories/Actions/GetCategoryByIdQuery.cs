using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using CodingJournal.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Categories.Actions;

public record GetCategoryByIdQuery(int Id) : IRequest<Result<CategoryDto>>;

public class GetCategoryByIdQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor) 
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result<CategoryDto>.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;

        var category = await context.Categories
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        if (category == null)
        {
            return Result<CategoryDto>.Failure("Category not found.");
        }
        
        return Result<CategoryDto>.Success(new CategoryDto(category.Id, category.Name));
    }
}