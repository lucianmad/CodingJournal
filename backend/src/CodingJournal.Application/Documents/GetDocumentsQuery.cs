using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Documents.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Documents;

public record GetDocumentsQuery : IRequest<List<DocumentDto>>;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDocumentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Documents.Include(d => d.User)
            .Include(d => d.Category)
            .Select(d => new DocumentDto(d.Id, 
                d.Title, 
                d.Content, 
                d.CreatedAt, 
                d.UpdatedAt, 
                d.User.Email, 
                d.Category != null ? d.Category.Name : null))
            .ToListAsync(cancellationToken);
    }
}