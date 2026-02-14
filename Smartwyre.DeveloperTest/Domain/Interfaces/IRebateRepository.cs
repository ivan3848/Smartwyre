using Smartwyre.DeveloperTest.Domain.Entities;

namespace Smartwyre.DeveloperTest.Domain.Interfaces;

/// <summary>
/// Repository interface for Rebate persistence - Following Repository Pattern
/// Part of Domain layer - defines contract but implementation is in Infrastructure
/// </summary>
public interface IRebateRepository
{
    Rebate GetById(string rebateIdentifier);
    void SaveCalculationResult(Rebate rebate, decimal rebateAmount);
}
