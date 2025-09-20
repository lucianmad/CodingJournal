using CodingJournal.Application.Features.Documents.Actions;
using FluentValidation;

namespace CodingJournal.Application.Features.Documents.Validators;

public class CreateDocumentValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters.");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");
    }
}