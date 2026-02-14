using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Application.Interfaces;

/// <summary>
/// Factory interface for obtaining rebate calculation strategies
/// Part of Application layer - abstracts strategy selection
/// </summary>
public interface IRebateCalculationStrategyFactory
{
    IRebateCalculationStrategy GetStrategy(IncentiveType incentiveType);
}
