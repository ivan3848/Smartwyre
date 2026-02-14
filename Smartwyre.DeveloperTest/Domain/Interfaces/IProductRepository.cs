using Smartwyre.DeveloperTest.Domain.Entities;

namespace Smartwyre.DeveloperTest.Domain.Interfaces;

/// <summary>
/// Repository interface for Product persistence - Following Repository Pattern
/// Part of Domain layer - defines contract but implementation is in Infrastructure
/// </summary>
public interface IProductRepository
{
    Product GetById(string productIdentifier);
}
