using MediatR;

namespace AccountingApp.Application.AccountEntries.Queries.GetEntries;

public record GetEntriesQuery : IRequest<List<AccountEntryDto>>;
