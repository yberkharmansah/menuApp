using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;
using MenuApp.Api.Domain.Enums;

namespace MenuApp.Api.Application.Services;

public class PublicMenuService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public PublicMenuService(
        IBusinessRepository businessRepository,
        IMenuRepository menuRepository,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _businessRepository = businessRepository;
        _menuRepository = menuRepository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public PublicMenuSnapshot? GetSnapshot(string businessSlug, string? token)
    {
        var business = _businessRepository.GetBySlug(businessSlug);
        if (business is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(business.PublicToken) && token != business.PublicToken)
        {
            return null;
        }

        var menus = _menuRepository.ListByBusiness(business.Id).Where(m => m.IsPublished).ToList();
        if (menus.Count == 0)
        {
            return new PublicMenuSnapshot(business, Array.Empty<Menu>(), Array.Empty<Category>(), Array.Empty<Product>());
        }

        var menuIds = menus.Select(m => m.Id).ToHashSet();
        var categories = menus.SelectMany(m => _categoryRepository.ListByMenu(m.Id))
            .Where(c => !c.IsHidden)
            .OrderBy(c => c.SortOrder)
            .ToList();
        var categoryIds = categories.Select(c => c.Id).ToHashSet();

        var products = menus.SelectMany(m => _productRepository.ListByMenu(m.Id))
            .Where(p => categoryIds.Contains(p.CategoryId) && p.Visibility == ProductVisibility.Active)
            .OrderBy(p => p.SortOrder)
            .ToList();

        return new PublicMenuSnapshot(business, menus, categories, products);
    }
}

public record PublicMenuSnapshot(
    Business Business,
    IReadOnlyCollection<Menu> Menus,
    IReadOnlyCollection<Category> Categories,
    IReadOnlyCollection<Product> Products
);
