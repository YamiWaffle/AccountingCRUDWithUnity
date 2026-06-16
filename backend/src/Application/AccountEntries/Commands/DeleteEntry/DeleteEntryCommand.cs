using MediatR;

namespace AccountingApp.Application.AccountEntries.Commands.DeleteEntry;

public record DeleteEntryCommand(int Id) : IRequest;
