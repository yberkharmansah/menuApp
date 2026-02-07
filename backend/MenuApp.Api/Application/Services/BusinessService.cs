using MenuApp.Api.Application.Interfaces;
using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Services;

public class BusinessService
{
    private readonly IBusinessRepository _businessRepository;

    public BusinessService(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public Business Register(string businessName)
    {
        var slug = Slugify(businessName);
        var business = new Business(
            Guid.NewGuid(),
            businessName,
            slug,
            Guid.NewGuid().ToString("N")[..6],
            ThemeSettings.Default,
            DateTimeOffset.UtcNow
        );

        return _businessRepository.Add(business);
    }

    public Business? Login(string slug)
    {
        return _businessRepository.GetBySlug(slug);
    }

    public Business? GetById(Guid id)
    {
        return _businessRepository.GetById(id);
    }

    public IReadOnlyCollection<Business> List()
    {
        return _businessRepository.List();
    }

    public Business? UpdateTheme(Guid businessId, ThemeSettings theme)
    {
        var existing = _businessRepository.GetById(businessId);
        if (existing is null)
        {
            return null;
        }

        var updated = existing with { ThemeSettings = theme };
        return _businessRepository.Update(updated);
    }

    private static string Slugify(string input)
    {
        return input.Trim().ToLowerInvariant().Replace(" ", "-");
    }
}
