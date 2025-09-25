using System.Security.Claims;
using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Common;
using CodingJournal.Application.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Features.Documents.Actions;

public record DeleteDocumentCommand(int Id) : IRequest<Result>;

public class DeleteDocumentCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteDocumentCommand, Result>
{
    public async Task<Result> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var userIdResult = httpContextAccessor.HttpContext.GetCurrentUserId();
        if (!userIdResult.IsSuccess)
        {
            return Result.Failure(userIdResult.Errors);
        }
        
        var userId = userIdResult.Value;
        
        var document = await context.Documents.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == userId, cancellationToken);
        if (document == null)
        {
            return Result.Failure("Document not found.");
        }
        
        context.Documents.Remove(document);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}