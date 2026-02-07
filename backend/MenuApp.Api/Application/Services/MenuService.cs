using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Services;

public class MenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public Menu Create(Guid businessId, string name)
    {
        var menu = new Menu(Guid.NewGuid(), businessId, name, false, DateTimeOffset.UtcNow);
        return _menuRepository.Add(menu);
    }

    public Menu? Publish(Guid menuId)
    {
        var menu = _menuRepository.GetById(menuId);
        if (menu is null)
        {
            return null;
        }

        var updated = menu with { IsPublished = true };
        return _menuRepository.Update(updated);
    }

    public Menu? GetById(Guid menuId)
    {
        return _menuRepository.GetById(menuId);
    }

    public IReadOnlyCollection<Menu> ListByBusiness(Guid businessId)
    {
        return _menuRepository.ListByBusiness(businessId);
    }
}
