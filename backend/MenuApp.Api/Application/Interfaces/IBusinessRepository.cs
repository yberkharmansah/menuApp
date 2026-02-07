using MenuApp.Api.Domain.Entities;

namespace MenuApp.Api.Application.Interfaces;

public interface IBusinessRepository
{
    Business Add(Business business);
    Business? GetById(Guid id);
    Business? GetBySlug(string slug);
    Business Update(Business business);
    IReadOnlyCollection<Business> List();
}
