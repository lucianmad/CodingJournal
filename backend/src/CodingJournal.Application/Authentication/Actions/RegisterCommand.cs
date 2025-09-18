using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Authentication.DTOs;
using CodingJournal.Application.Common;
using CodingJournal.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CodingJournal.Application.Authentication.Actions;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password, string ConfirmPassword) : IRequest<Result<AuthResponseDto>>;

public class RegisterCommandHandler(IApplicationDbContext context, IJwtService jwtService, UserManager<User> userManager)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (context.Users.Any(u => u.Email == request.Email))
        {
            return Result<AuthResponseDto>.Failure("Email already exists.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            return Result<AuthResponseDto>.Failure("Passwords do not match.");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            EmailConfirmed = true,
            UserName = request.Email,
            CreatedAt = DateTime.UtcNow,
        };
        
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Result<AuthResponseDto>.Failure("Failed to register user.");
        }
        
        var token = jwtService.GenerateToken(user.Id, user.Email);
        
        return Result<AuthResponseDto>.Success(new AuthResponseDto(user.Id, user.Email, token, user.FirstName, user.LastName));
    }
}