using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Categories.Actions;

public record DeleteCategoryCommand(int Id) : IRequest<Result>;

public class DeleteCategoryCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;

        var category = await context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userId, cancellationToken);
        
        if (category == null)
        {
            return Result.Failure("Category not found.");
        }
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}