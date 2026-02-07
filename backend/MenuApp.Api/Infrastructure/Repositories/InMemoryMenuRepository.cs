using System.Collections.Concurrent;
using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Infrastructure.Repositories;

public class InMemoryMenuRepository : IMenuRepository
{
    private readonly ConcurrentDictionary<Guid, Menu> _menus = new();

    public Menu Add(Menu menu)
    {
        _menus[menu.Id] = menu;
        return menu;
    }

    public Menu? GetById(Guid id)
    {
        _menus.TryGetValue(id, out var menu);
        return menu;
    }

    public IReadOnlyCollection<Menu> ListByBusiness(Guid businessId)
    {
        return _menus.Values.Where(m => m.BusinessId == businessId).ToList();
    }

    public Menu Update(Menu menu)
    {
        _menus[menu.Id] = menu;
        return menu;
    }
}
