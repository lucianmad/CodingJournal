using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using CodingJournal.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CodingJournal.Application.Features.Categories.Actions;

public record CreateCategoryCommand(string Name) : IRequest<Result<int>>;

public class CreateCategoryCommandHandler(
    IApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IValidator<CreateCategoryCommand> validator)
    : IRequestHandler<CreateCategoryCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result<int>.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<int>.Failure(errors);
        }

        var exists = context.Categories.Any(x => x.Name == request.Name && x.UserId == userId);
        if (exists)
        {
            return Result<int>.Failure("Category with the same name already exists.");
        }

        var category = new Category
        {
            Name = request.Name,
            UserId = userId
        };
        
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result<int>.Success(category.Id);
    }
}