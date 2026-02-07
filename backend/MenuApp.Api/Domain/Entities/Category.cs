namespace MenuApp.Api.Domain.Entities;

public record Category(
    Guid Id,
    Guid MenuId,
    string Name,
    int SortOrder,
    bool IsHidden,
    DateTimeOffset CreatedAt
);
