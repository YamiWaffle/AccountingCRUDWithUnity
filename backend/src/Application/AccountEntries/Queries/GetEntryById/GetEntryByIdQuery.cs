using AccountingApp.Application.AccountEntries.Queries.GetEntries;
using MediatR;

namespace AccountingApp.Application.AccountEntries.Queries.GetEntryById;

public record GetEntryByIdQuery(int Id) : IRequest<AccountEntryDto>;
