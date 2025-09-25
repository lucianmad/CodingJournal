using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Categories.Actions;

public record UpdateCategoryCommand(int Id, string Name) : IRequest<Result>;

public class UpdateCategoryCommandHandler(
    IApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IValidator<UpdateCategoryCommand> validator)
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            return Result.Failure(errors);
        }

        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userId, cancellationToken);
        if (category == null)
        {
            return Result.Failure("Category not found.");
        }

        var exists = await context.Categories.AnyAsync(x => x.Name == request.Name && x.UserId == userId && x.Id != category.Id,
                cancellationToken);
        if (exists)
        {
            return Result.Failure("Category with same name already exists.");
        }

        category.Name = request.Name;
        
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}