using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Features.Authentication.DTOs;
using CodingJournal.Application.Common;
using CodingJournal.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CodingJournal.Application.Features.Authentication.Actions;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password, string ConfirmPassword) : IRequest<Result<AuthResponseDto>>;

public class RegisterCommandHandler(IApplicationDbContext context, IJwtService jwtService, UserManager<User> userManager, IValidator<RegisterCommand> validator)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Result<AuthResponseDto>.Failure(errors);
        }
        
        if (context.Users.Any(u => u.Email == request.Email))
        {
            return Result<AuthResponseDto>.Failure("Email already exists.");
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