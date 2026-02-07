using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Interfaces;

public interface IProductRepository
{
    Product Add(Product product);
    Product? GetById(Guid id);
    IReadOnlyCollection<Product> ListByMenu(Guid menuId);
    IReadOnlyCollection<Product> ListByCategory(Guid categoryId);
    Product Update(Product product);
}
