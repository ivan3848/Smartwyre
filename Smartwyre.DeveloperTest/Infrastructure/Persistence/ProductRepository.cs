using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Interfaces;
using Smartwyre.DeveloperTest.Infrastructure.Data;

namespace Smartwyre.DeveloperTest.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for Product persistence
/// Clean Architecture - Infrastructure layer implements domain interfaces
/// Uses mock data for demonstration - in production would connect to actual database
/// </summary>
public class ProductRepository : IProductRepository
{
    public Product GetById(string productIdentifier)
    {
        // Get product from mock data store
        if (MockDataStore.Products.TryGetValue(productIdentifier, out var product))
        {
            return product;
        }

        // Return null if product not found
        return null;
    }
}
