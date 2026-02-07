using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;
using MenuApp.Api.Domain.Enums;

namespace MenuApp.Api.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Product Create(
        Guid menuId,
        Guid categoryId,
        string name,
        string? description,
        decimal price,
        string currency,
        string? imageUrl,
        IReadOnlyList<string> tags,
        IReadOnlyList<string> allergens
    )
    {
        var sortOrder = _productRepository.ListByCategory(categoryId).Count + 1;
        var product = new Product(
            Guid.NewGuid(),
            menuId,
            categoryId,
            name,
            description,
            price,
            currency,
            imageUrl,
            tags,
            allergens,
            sortOrder,
            ProductVisibility.Active,
            DateTimeOffset.UtcNow
        );

        return _productRepository.Add(product);
    }

    public Product? Update(
        Guid id,
        string? name,
        string? description,
        decimal? price,
        string? currency,
        string? imageUrl,
        IReadOnlyList<string>? tags,
        IReadOnlyList<string>? allergens,
        int? sortOrder,
        ProductVisibility? visibility,
        Guid? categoryId
    )
    {
        var product = _productRepository.GetById(id);
        if (product is null)
        {
            return null;
        }

        var updated = product with
        {
            Name = name ?? product.Name,
            Description = description ?? product.Description,
            Price = price ?? product.Price,
            Currency = currency ?? product.Currency,
            ImageUrl = imageUrl ?? product.ImageUrl,
            Tags = tags ?? product.Tags,
            Allergens = allergens ?? product.Allergens,
            SortOrder = sortOrder ?? product.SortOrder,
            Visibility = visibility ?? product.Visibility,
            CategoryId = categoryId ?? product.CategoryId
        };

        return _productRepository.Update(updated);
    }

    public Product? UpdateVisibility(Guid id, ProductVisibility visibility)
    {
        var product = _productRepository.GetById(id);
        if (product is null)
        {
            return null;
        }

        var updated = product with { Visibility = visibility };
        return _productRepository.Update(updated);
    }

    public IReadOnlyCollection<Product> ListByMenu(Guid menuId)
    {
        return _productRepository.ListByMenu(menuId);
    }
}
