using MenuApp.Api.Domain.Enums;

namespace MenuApp.Api.Api.Contracts;

public record CreateProductRequest(
    Guid MenuId,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    string? ImageUrl,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> Allergens
);

public record UpdateProductRequest(
    string? Name,
    string? Description,
    decimal? Price,
    string? Currency,
    string? ImageUrl,
    IReadOnlyList<string>? Tags,
    IReadOnlyList<string>? Allergens,
    int? SortOrder,
    ProductVisibility? Visibility,
    Guid? CategoryId
);

public record UpdateVisibilityRequest(ProductVisibility Visibility);
