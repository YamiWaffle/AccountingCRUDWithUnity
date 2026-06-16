using AccountingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Application.Common;

public interface IAccountingDbContext
{
    DbSet<AccountEntry> AccountEntries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
