using System.Collections.Concurrent;
using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Infrastructure.Repositories;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly ConcurrentDictionary<Guid, Category> _categories = new();

    public Category Add(Category category)
    {
        _categories[category.Id] = category;
        return category;
    }

    public Category? GetById(Guid id)
    {
        _categories.TryGetValue(id, out var category);
        return category;
    }

    public IReadOnlyCollection<Category> ListByMenu(Guid menuId)
    {
        return _categories.Values.Where(c => c.MenuId == menuId).ToList();
    }

    public Category Update(Category category)
    {
        _categories[category.Id] = category;
        return category;
    }
}
