using CodingJournal.Application.Features.Documents.Actions;
using FluentValidation;

namespace CodingJournal.Application.Features.Documents.Validators;

public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters.");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");
    }
}