using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Domain.Enums;

namespace Smartwyre.DeveloperTest.Application.Strategies;

/// <summary>
/// Factory for selecting the appropriate rebate calculation strategy
/// Clean Architecture - Application layer orchestrates strategy selection
/// </summary>
public class RebateCalculationStrategyFactory : IRebateCalculationStrategyFactory
{
    private readonly IEnumerable<IRebateCalculationStrategy> _strategies;

    public RebateCalculationStrategyFactory(IEnumerable<IRebateCalculationStrategy> strategies)
    {
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    public IRebateCalculationStrategy GetStrategy(IncentiveType incentiveType)
    {
        var strategy = _strategies.FirstOrDefault(s => s.SupportedIncentiveType == incentiveType);
        
        if (strategy == null)
        {
            throw new NotSupportedException($"Incentive type '{incentiveType}' is not supported.");
        }

        return strategy;
    }
}
