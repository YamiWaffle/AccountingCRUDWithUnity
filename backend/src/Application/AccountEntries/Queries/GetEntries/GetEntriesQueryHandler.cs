using AccountingApp.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Application.AccountEntries.Queries.GetEntries;

public class GetEntriesQueryHandler : IRequestHandler<GetEntriesQuery, List<AccountEntryDto>>
{
    private readonly IAccountingDbContext _context;

    public GetEntriesQueryHandler(IAccountingDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountEntryDto>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.AccountEntries
            .OrderByDescending(e => e.Date)
            .Select(e => new AccountEntryDto(e.Id, e.Amount, e.Category, e.Note, e.Date, e.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
