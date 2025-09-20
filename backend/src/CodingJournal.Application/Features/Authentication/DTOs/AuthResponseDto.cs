namespace CodingJournal.Application.Features.Authentication.DTOs;

public record AuthResponseDto(string UserId, string Email, string Token, string FirstName, string LastName);