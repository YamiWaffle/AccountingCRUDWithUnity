using AccountingApp.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Application.AccountEntries.Commands.DeleteEntry;

public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand>
{
    private readonly IAccountingDbContext _context;

    public DeleteEntryCommandHandler(IAccountingDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.AccountEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AccountEntry {request.Id} not found.");

        _context.AccountEntries.Remove(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
