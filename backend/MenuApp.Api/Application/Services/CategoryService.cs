using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Category Create(Guid menuId, string name)
    {
        var sortOrder = _categoryRepository.ListByMenu(menuId).Count + 1;
        var category = new Category(Guid.NewGuid(), menuId, name, sortOrder, false, DateTimeOffset.UtcNow);
        return _categoryRepository.Add(category);
    }

    public Category? Update(Guid id, string? name, int? sortOrder, bool? isHidden)
    {
        var category = _categoryRepository.GetById(id);
        if (category is null)
        {
            return null;
        }

        var updated = category with
        {
            Name = name ?? category.Name,
            SortOrder = sortOrder ?? category.SortOrder,
            IsHidden = isHidden ?? category.IsHidden
        };

        return _categoryRepository.Update(updated);
    }

    public IReadOnlyCollection<Category> ListByMenu(Guid menuId)
    {
        return _categoryRepository.ListByMenu(menuId);
    }
}
