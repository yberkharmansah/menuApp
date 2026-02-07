using System.Collections.Concurrent;
using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Product Add(Product product)
    {
        _products[product.Id] = product;
        return product;
    }

    public Product? GetById(Guid id)
    {
        _products.TryGetValue(id, out var product);
        return product;
    }

    public IReadOnlyCollection<Product> ListByMenu(Guid menuId)
    {
        return _products.Values.Where(p => p.MenuId == menuId).ToList();
    }

    public IReadOnlyCollection<Product> ListByCategory(Guid categoryId)
    {
        return _products.Values.Where(p => p.CategoryId == categoryId).ToList();
    }

    public Product Update(Product product)
    {
        _products[product.Id] = product;
        return product;
    }
}
