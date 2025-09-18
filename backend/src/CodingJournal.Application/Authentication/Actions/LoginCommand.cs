using CodingJournal.Application.Abstractions;
using CodingJournal.Application.Authentication.DTOs;
using CodingJournal.Application.Common;
using CodingJournal.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CodingJournal.Application.Authentication.Actions;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;

public class LoginCommandHandler(IJwtService jwtService, UserManager<User> userManager) 
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password.");
        }
        
        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password.");
        }
        
        var token = jwtService.GenerateToken(user.Id, user.Email!);
        
        return Result<AuthResponseDto>.Success(new AuthResponseDto(user.Id, user.Email!, token, user.FirstName!, user.LastName!));
    }
}