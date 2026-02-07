using System.Text.Json.Serialization;
using MenuApp.Api.Api.Contracts;
using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Application.Services;
using MenuApp.Api.Domain.Entities;
using MenuApp.Api.Domain.Enums;
using MenuApp.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<IBusinessRepository, InMemoryBusinessRepository>();
builder.Services.AddSingleton<IMenuRepository, InMemoryMenuRepository>();
builder.Services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

builder.Services.AddSingleton<BusinessService>();
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<PublicMenuService>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "MenuApp API", version = "0.2.0" }));

var authGroup = app.MapGroup("/api/auth");

authGroup.MapPost("/register", (RegisterRequest request, BusinessService businessService) =>
{
    var business = businessService.Register(request.BusinessName);
    return Results.Ok(new AuthResponse(business.Id, business.Slug, "demo-token"));
});

authGroup.MapPost("/login", (LoginRequest request, BusinessService businessService) =>
{
    var business = businessService.Login(request.BusinessSlug);
    return business is null
        ? Results.NotFound(new { message = "Business not found" })
        : Results.Ok(new AuthResponse(business.Id, business.Slug, "demo-token"));
});

var adminGroup = app.MapGroup("/api/admin");

adminGroup.MapGet("/businesses", (BusinessService businessService) =>
{
    return Results.Ok(businessService.List());
});

adminGroup.MapPut("/businesses/{businessId:guid}/theme", (Guid businessId, UpdateThemeRequest request, BusinessService businessService) =>
{
    var business = businessService.GetById(businessId);
    if (business is null)
    {
        return Results.NotFound();
    }

    var updatedTheme = business.ThemeSettings with
    {
        LogoUrl = request.LogoUrl ?? business.ThemeSettings.LogoUrl,
        PrimaryColor = request.PrimaryColor ?? business.ThemeSettings.PrimaryColor,
        SecondaryColor = request.SecondaryColor ?? business.ThemeSettings.SecondaryColor,
        Font = request.Font ?? business.ThemeSettings.Font,
        Currency = request.Currency ?? business.ThemeSettings.Currency,
        Language = request.Language ?? business.ThemeSettings.Language
    };

    var updatedBusiness = businessService.UpdateTheme(businessId, updatedTheme);
    return Results.Ok(updatedBusiness);
});

adminGroup.MapGet("/menus", (Guid businessId, MenuService menuService) =>
{
    if (businessId == Guid.Empty)
    {
        return Results.BadRequest(new { message = "businessId is required" });
    }

    return Results.Ok(menuService.ListByBusiness(businessId));
});

adminGroup.MapPost("/menus", (CreateMenuRequest request, MenuService menuService) =>
{
    var menu = menuService.Create(request.BusinessId, request.Name);
    return Results.Created($"/api/admin/menus/{menu.Id}", menu);
});

adminGroup.MapGet("/menus/{menuId:guid}", (Guid menuId, MenuService menuService) =>
{
    var menu = menuService.GetById(menuId);
    return menu is null ? Results.NotFound() : Results.Ok(menu);
});

adminGroup.MapPost("/menus/{menuId:guid}/publish", (Guid menuId, MenuService menuService) =>
{
    var menu = menuService.Publish(menuId);
    return menu is null ? Results.NotFound() : Results.Ok(menu);
});

adminGroup.MapGet("/categories", (Guid menuId, CategoryService categoryService) =>
{
    if (menuId == Guid.Empty)
    {
        return Results.BadRequest(new { message = "menuId is required" });
    }

    return Results.Ok(categoryService.ListByMenu(menuId));
});

adminGroup.MapPost("/categories", (CreateCategoryRequest request, CategoryService categoryService) =>
{
    var category = categoryService.Create(request.MenuId, request.Name);
    return Results.Created($"/api/admin/categories/{category.Id}", category);
});

adminGroup.MapPatch("/categories/{id:guid}", (Guid id, UpdateCategoryRequest request, CategoryService categoryService) =>
{
    var category = categoryService.Update(id, request.Name, request.SortOrder, request.IsHidden);
    return category is null ? Results.NotFound() : Results.Ok(category);
});

adminGroup.MapGet("/products", (Guid menuId, ProductService productService) =>
{
    if (menuId == Guid.Empty)
    {
        return Results.BadRequest(new { message = "menuId is required" });
    }

    return Results.Ok(productService.ListByMenu(menuId));
});

adminGroup.MapPost("/products", (CreateProductRequest request, ProductService productService) =>
{
    var product = productService.Create(
        request.MenuId,
        request.CategoryId,
        request.Name,
        request.Description,
        request.Price,
        request.Currency,
        request.ImageUrl,
        request.Tags,
        request.Allergens
    );

    return Results.Created($"/api/admin/products/{product.Id}", product);
});

adminGroup.MapPatch("/products/{id:guid}", (Guid id, UpdateProductRequest request, ProductService productService) =>
{
    var product = productService.Update(
        id,
        request.Name,
        request.Description,
        request.Price,
        request.Currency,
        request.ImageUrl,
        request.Tags,
        request.Allergens,
        request.SortOrder,
        request.Visibility,
        request.CategoryId
    );

    return product is null ? Results.NotFound() : Results.Ok(product);
});

adminGroup.MapPost("/products/{id:guid}/visibility", (Guid id, UpdateVisibilityRequest request, ProductService productService) =>
{
    var product = productService.UpdateVisibility(id, request.Visibility);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

var publicGroup = app.MapGroup("/api/public");

publicGroup.MapGet("/menus/{businessSlug}", (string businessSlug, string? t, PublicMenuService publicMenuService) =>
{
    var snapshot = publicMenuService.GetSnapshot(businessSlug, t);
    return snapshot is null ? Results.NotFound() : Results.Ok(snapshot);
});

app.Run();
