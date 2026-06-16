namespace AccountingApp.Application.AccountEntries.Queries.GetEntries;

public record AccountEntryDto(
    int Id,
    decimal Amount,
    string Category,
    string Note,
    DateTime Date,
    DateTime CreatedAt
);
