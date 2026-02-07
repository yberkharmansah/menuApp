using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Interfaces;

public interface ICategoryRepository
{
    Category Add(Category category);
    Category? GetById(Guid id);
    IReadOnlyCollection<Category> ListByMenu(Guid menuId);
    Category Update(Category category);
}
