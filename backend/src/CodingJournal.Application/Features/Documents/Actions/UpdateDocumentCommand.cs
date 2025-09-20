using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Documents.Actions;

public record UpdateDocumentCommand(int Id, string Title, string Content, int? CategoryId) : IRequest<Result>;

public class UpdateDocumentCommandHandler(IApplicationDbContext context, IValidator<UpdateDocumentCommand> validator, IHttpContextAccessor httpContextAccessor) 
    : IRequestHandler<UpdateDocumentCommand, Result>
{
    public async Task<Result> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("User not found.");
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result.Failure(errors);
        }
        
        var document = await context.Documents.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userId, cancellationToken);
        if (document == null)
        {
            return Result.Failure("Document not found.");
        }
        
        
        var exists = await context.Documents.AnyAsync(d => d.Title == request.Title && d.UserId == userId && d.Id != document.Id, cancellationToken);
        if (exists)
        {
            return Result.Failure("Document with same title already exists.");   
        }
        
        document.Title = request.Title;
        document.Content = request.Content;
        document.CategoryId = request.CategoryId;
        document.UpdatedAt = DateTime.UtcNow;
        
        await context.SaveChangesAsync(cancellationToken);
            
        return Result.Success();
    }
}