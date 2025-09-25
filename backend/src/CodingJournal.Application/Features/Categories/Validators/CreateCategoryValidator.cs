using CodingJournal.Application.Features.Categories.Actions;
using FluentValidation;

namespace CodingJournal.Application.Features.Categories.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.");
    }
}