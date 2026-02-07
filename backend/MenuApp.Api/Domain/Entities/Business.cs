namespace MenuApp.Api.Domain.Entities;

public record Business(
    Guid Id,
    string Name,
    string Slug,
    string? PublicToken,
    ThemeSettings ThemeSettings,
    DateTimeOffset CreatedAt
);
