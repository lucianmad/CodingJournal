using CodingJournal.Application.Features.Authentication.Actions;
using FluentValidation;

namespace CodingJournal.Application.Features.Authentication.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}