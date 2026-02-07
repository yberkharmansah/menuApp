namespace MenuApp.Api.Domain.Entities;

public record Menu(
    Guid Id,
    Guid BusinessId,
    string Name,
    bool IsPublished,
    DateTimeOffset CreatedAt
);
