using AccountingApp.Application.Common;
using AccountingApp.Domain.Entities;
using MediatR;

namespace AccountingApp.Application.AccountEntries.Commands.CreateEntry;

public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, int>
{
    private readonly IAccountingDbContext _context;

    public CreateEntryCommandHandler(IAccountingDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = new AccountEntry
        {
            Amount = request.Amount,
            Category = request.Category,
            Note = request.Note,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.AccountEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}
