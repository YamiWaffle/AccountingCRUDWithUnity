using MediatR;

namespace AccountingApp.Application.AccountEntries.Commands.CreateEntry;

public record CreateEntryCommand(decimal Amount, string Category, string Note, DateTime Date)
    : IRequest<int>;
