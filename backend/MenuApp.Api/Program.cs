using System.Collections.Concurrent;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "MenuApp API", version = "0.1.0" }));

var store = new InMemoryStore();

app.MapPost("/api/auth/register", (RegisterRequest request) =>
{
    var business = store.CreateBusiness(request.BusinessName);
    return Results.Ok(new AuthResponse(business.Id, business.Slug, "demo-token"));
});

app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    var business = store.GetBusinessBySlug(request.BusinessSlug);
    return business is null
        ? Results.NotFound(new { message = "Business not found" })
        : Results.Ok(new AuthResponse(business.Id, business.Slug, "demo-token"));
});

app.MapGet("/api/admin/menus", () => Results.Ok(store.Menus));

app.MapPost("/api/admin/menus", (CreateMenuRequest request) =>
{
    var menu = store.CreateMenu(request.BusinessId, request.Name);
    return Results.Created($"/api/admin/menus/{menu.Id}", menu);
});

app.MapGet("/api/admin/menus/{menuId:guid}", (Guid menuId) =>
{
    var menu = store.Menus.FirstOrDefault(m => m.Id == menuId);
    return menu is null ? Results.NotFound() : Results.Ok(menu);
});

app.MapPost("/api/admin/categories", (CreateCategoryRequest request) =>
{
    var category = store.CreateCategory(request.MenuId, request.Name);
    return Results.Created($"/api/admin/categories/{category.Id}", category);
});

app.MapPatch("/api/admin/categories/{id:guid}", (Guid id, UpdateCategoryRequest request) =>
{
    var category = store.UpdateCategory(id, request);
    return category is null ? Results.NotFound() : Results.Ok(category);
});

app.MapPost("/api/admin/products", (CreateProductRequest request) =>
{
    var product = store.CreateProduct(request);
    return Results.Created($"/api/admin/products/{product.Id}", product);
});

app.MapPatch("/api/admin/products/{id:guid}", (Guid id, UpdateProductRequest request) =>
{
    var product = store.UpdateProduct(id, request);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/api/admin/products/{id:guid}/visibility", (Guid id, UpdateVisibilityRequest request) =>
{
    var product = store.UpdateVisibility(id, request.Visibility);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/api/admin/menus/{menuId:guid}/publish", (Guid menuId) =>
{
    var menu = store.PublishMenu(menuId);
    return menu is null ? Results.NotFound() : Results.Ok(menu);
});

app.MapGet("/api/public/menus/{businessSlug}", (string businessSlug, string? t) =>
{
    var response = store.GetPublicMenu(businessSlug, t);
    return response is null ? Results.NotFound() : Results.Ok(response);
});

app.Run();

record RegisterRequest(string BusinessName);
record LoginRequest(string BusinessSlug);
record AuthResponse(Guid BusinessId, string BusinessSlug, string Token);
record CreateMenuRequest(Guid BusinessId, string Name);
record CreateCategoryRequest(Guid MenuId, string Name);
record UpdateCategoryRequest(string? Name, int? SortOrder, bool? IsHidden);
record CreateProductRequest(Guid MenuId, Guid CategoryId, string Name, string? Description, decimal Price, string Currency, string? ImageUrl, string[] Tags, string[] Allergens);
record UpdateProductRequest(string? Name, string? Description, decimal? Price, string? Currency, string? ImageUrl, string[]? Tags, string[]? Allergens, int? SortOrder, ProductVisibility? Visibility, Guid? CategoryId);
record UpdateVisibilityRequest(ProductVisibility Visibility);

enum ProductVisibility
{
    Active,
    OutOfStock,
    Passive
}

record Business(Guid Id, string Name, string Slug, string? PublicToken);
record Menu(Guid Id, Guid BusinessId, string Name, bool IsPublished);
record Category(Guid Id, Guid MenuId, string Name, int SortOrder, bool IsHidden);
record Product(Guid Id, Guid MenuId, Guid CategoryId, string Name, string? Description, decimal Price, string Currency, string? ImageUrl, string[] Tags, string[] Allergens, int SortOrder, ProductVisibility Visibility);

class InMemoryStore
{
    private readonly ConcurrentDictionary<Guid, Business> _businesses = new();
    private readonly ConcurrentDictionary<Guid, Menu> _menus = new();
    private readonly ConcurrentDictionary<Guid, Category> _categories = new();
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public IEnumerable<Menu> Menus => _menus.Values;

    public Business CreateBusiness(string name)
    {
        var slug = name.Trim().ToLowerInvariant().Replace(" ", "-");
        var business = new Business(Guid.NewGuid(), name, slug, Guid.NewGuid().ToString("N")[..6]);
        _businesses[business.Id] = business;
        return business;
    }

    public Business? GetBusinessBySlug(string slug)
    {
        return _businesses.Values.FirstOrDefault(b => b.Slug == slug);
    }

    public Menu CreateMenu(Guid businessId, string name)
    {
        var menu = new Menu(Guid.NewGuid(), businessId, name, false);
        _menus[menu.Id] = menu;
        return menu;
    }

    public Category CreateCategory(Guid menuId, string name)
    {
        var sortOrder = _categories.Values.Count(c => c.MenuId == menuId) + 1;
        var category = new Category(Guid.NewGuid(), menuId, name, sortOrder, false);
        _categories[category.Id] = category;
        return category;
    }

    public Category? UpdateCategory(Guid id, UpdateCategoryRequest request)
    {
        if (!_categories.TryGetValue(id, out var category))
        {
            return null;
        }

        var updated = category with
        {
            Name = request.Name ?? category.Name,
            SortOrder = request.SortOrder ?? category.SortOrder,
            IsHidden = request.IsHidden ?? category.IsHidden
        };
        _categories[id] = updated;
        return updated;
    }

    public Product CreateProduct(CreateProductRequest request)
    {
        var sortOrder = _products.Values.Count(p => p.CategoryId == request.CategoryId) + 1;
        var product = new Product(
            Guid.NewGuid(),
            request.MenuId,
            request.CategoryId,
            request.Name,
            request.Description,
            request.Price,
            request.Currency,
            request.ImageUrl,
            request.Tags,
            request.Allergens,
            sortOrder,
            ProductVisibility.Active
        );
        _products[product.Id] = product;
        return product;
    }

    public Product? UpdateProduct(Guid id, UpdateProductRequest request)
    {
        if (!_products.TryGetValue(id, out var product))
        {
            return null;
        }

        var updated = product with
        {
            Name = request.Name ?? product.Name,
            Description = request.Description ?? product.Description,
            Price = request.Price ?? product.Price,
            Currency = request.Currency ?? product.Currency,
            ImageUrl = request.ImageUrl ?? product.ImageUrl,
            Tags = request.Tags ?? product.Tags,
            Allergens = request.Allergens ?? product.Allergens,
            SortOrder = request.SortOrder ?? product.SortOrder,
            Visibility = request.Visibility ?? product.Visibility,
            CategoryId = request.CategoryId ?? product.CategoryId
        };

        _products[id] = updated;
        return updated;
    }

    public Product? UpdateVisibility(Guid id, ProductVisibility visibility)
    {
        if (!_products.TryGetValue(id, out var product))
        {
            return null;
        }

        var updated = product with { Visibility = visibility };
        _products[id] = updated;
        return updated;
    }

    public Menu? PublishMenu(Guid menuId)
    {
        if (!_menus.TryGetValue(menuId, out var menu))
        {
            return null;
        }

        var updated = menu with { IsPublished = true };
        _menus[menuId] = updated;
        return updated;
    }

    public object? GetPublicMenu(string businessSlug, string? token)
    {
        var business = GetBusinessBySlug(businessSlug);
        if (business is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(business.PublicToken) && token != business.PublicToken)
        {
            return null;
        }

        var menus = _menus.Values.Where(m => m.BusinessId == business.Id && m.IsPublished).ToList();
        var categories = _categories.Values.Where(c => menus.Any(m => m.Id == c.MenuId) && !c.IsHidden).ToList();
        var products = _products.Values.Where(p => categories.Any(c => c.Id == p.CategoryId) && p.Visibility == ProductVisibility.Active).ToList();

        return new
        {
            business.Id,
            business.Name,
            business.Slug,
            menus,
            categories,
            products
        };
    }
}
