using AccountingApp.Application.AccountEntries.Queries.GetEntries;
using AccountingApp.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Application.AccountEntries.Queries.GetEntryById;

public class GetEntryByIdQueryHandler : IRequestHandler<GetEntryByIdQuery, AccountEntryDto>
{
    private readonly IAccountingDbContext _context;

    public GetEntryByIdQueryHandler(IAccountingDbContext context)
    {
        _context = context;
    }

    public async Task<AccountEntryDto> Handle(GetEntryByIdQuery request, CancellationToken cancellationToken)
    {
        var entry = await _context.AccountEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AccountEntry {request.Id} not found.");

        return new AccountEntryDto(entry.Id, entry.Amount, entry.Category, entry.Note, entry.Date, entry.CreatedAt);
    }
}
