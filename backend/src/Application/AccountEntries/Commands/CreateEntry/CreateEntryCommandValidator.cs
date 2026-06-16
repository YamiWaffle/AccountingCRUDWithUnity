using FluentValidation;

namespace AccountingApp.Application.AccountEntries.Commands.CreateEntry;

public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.Amount).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Note).MaximumLength(500);
        RuleFor(x => x.Date).NotEmpty();
    }
}
