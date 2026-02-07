using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Interfaces;

public interface IMenuRepository
{
    Menu Add(Menu menu);
    Menu? GetById(Guid id);
    IReadOnlyCollection<Menu> ListByBusiness(Guid businessId);
    Menu Update(Menu menu);
}
