using CodingJournal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodingJournal.Application.Abstractions;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Document> Documents { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}