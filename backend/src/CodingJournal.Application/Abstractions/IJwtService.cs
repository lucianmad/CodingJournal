namespace CodingJournal.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(string userId, string email);
}