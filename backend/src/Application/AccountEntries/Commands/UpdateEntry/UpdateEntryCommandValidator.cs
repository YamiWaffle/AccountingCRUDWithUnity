using FluentValidation;

namespace AccountingApp.Application.AccountEntries.Commands.UpdateEntry;

public class UpdateEntryCommandValidator : AbstractValidator<UpdateEntryCommand>
{
    public UpdateEntryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Amount).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Note).MaximumLength(500);
        RuleFor(x => x.Date).NotEmpty();
    }
}
