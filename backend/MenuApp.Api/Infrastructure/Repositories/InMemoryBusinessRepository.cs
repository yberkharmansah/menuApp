using System.Collections.Concurrent;
using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Infrastructure.Repositories;

public class InMemoryBusinessRepository : IBusinessRepository
{
    private readonly ConcurrentDictionary<Guid, Business> _businesses = new();

    public Business Add(Business business)
    {
        _businesses[business.Id] = business;
        return business;
    }

    public Business? GetById(Guid id)
    {
        _businesses.TryGetValue(id, out var business);
        return business;
    }

    public Business? GetBySlug(string slug)
    {
        return _businesses.Values.FirstOrDefault(b => b.Slug == slug);
    }

    public Business Update(Business business)
    {
        _businesses[business.Id] = business;
        return business;
    }

    public IReadOnlyCollection<Business> List()
    {
        return _businesses.Values.ToList();
    }
}
