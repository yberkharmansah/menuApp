using MenuApp.Api.Domain.Enums;

namespace MenuApp.Api.Domain.Entities;

public record Product(
    Guid Id,
    Guid MenuId,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    string? ImageUrl,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> Allergens,
    int SortOrder,
    ProductVisibility Visibility,
    DateTimeOffset CreatedAt
);
