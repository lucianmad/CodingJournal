using System.Reflection.Metadata;
using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Document = CodingJournal.Domain.Entities.Document;

namespace CodingJournal.Application.Features.Documents.Actions;

public record CreateDocumentCommand(string Title, string Content, int? CategoryId) : IRequest<Result<int>>;

public class CreateDocumentCommandHandler(IApplicationDbContext context, IValidator<CreateDocumentCommand> validator, IHttpContextAccessor httpContextAccessor) 
    : IRequestHandler<CreateDocumentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
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
        
        var exists = context.Documents.Any(d => d.Title == request.Title && d.UserId == userId);
        if (exists)
        {
            return Result<int>.Failure("Document with same title already exists.");
        }

        var document = new Document
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Documents.Add(document);
        await context.SaveChangesAsync(cancellationToken);
            
        return Result<int>.Success(document.Id);
    }
}