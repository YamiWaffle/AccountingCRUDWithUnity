using MediatR;

namespace AccountingApp.Application.AccountEntries.Commands.UpdateEntry;

public record UpdateEntryCommand(int Id, decimal Amount, string Category, string Note, DateTime Date)
    : IRequest;
