using System.Collections.Generic;
using AccountingApp.Api.Models;
using R3;

namespace AccountingApp.Store
{
    public sealed class AccountingStore
    {
        public readonly ReactiveProperty<List<AccountEntryDto>> Entries = new(new List<AccountEntryDto>());

        public readonly ReactiveProperty<bool> IsLoading = new(false);

        public readonly ReactiveProperty<bool> IsFormVisible = new(false);

        public readonly ReactiveProperty<AccountEntryDto> EditingEntry = new(null);
    }
}
