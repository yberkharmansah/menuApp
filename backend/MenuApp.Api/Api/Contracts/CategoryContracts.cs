namespace MenuApp.Api.Api.Contracts;

public record CreateCategoryRequest(Guid MenuId, string Name);
public record UpdateCategoryRequest(string? Name, int? SortOrder, bool? IsHidden);
