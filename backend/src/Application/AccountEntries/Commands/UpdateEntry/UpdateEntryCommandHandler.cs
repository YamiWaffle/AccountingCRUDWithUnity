using AccountingApp.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Application.AccountEntries.Commands.UpdateEntry;

public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand>
{
    private readonly IAccountingDbContext _context;

    public UpdateEntryCommandHandler(IAccountingDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.AccountEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AccountEntry {request.Id} not found.");

        entry.Amount = request.Amount;
        entry.Category = request.Category;
        entry.Note = request.Note;
        entry.Date = request.Date;
        entry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
