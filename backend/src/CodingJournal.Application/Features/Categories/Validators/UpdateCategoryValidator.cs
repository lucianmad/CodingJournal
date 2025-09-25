using CodingJournal.Application.Features.Categories.Actions;
using FluentValidation;

namespace CodingJournal.Application.Features.Categories.Validators;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.");
    }
}