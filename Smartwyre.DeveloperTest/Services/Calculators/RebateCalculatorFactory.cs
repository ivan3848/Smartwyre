using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public class RebateCalculatorFactory : IRebateCalculatorFactory
{
    private readonly IEnumerable<IRebateCalculator> _calculators;

    public RebateCalculatorFactory(IEnumerable<IRebateCalculator> calculators)
    {
        _calculators = calculators ?? throw new ArgumentNullException(nameof(calculators));
    }

    public IRebateCalculator GetCalculator(IncentiveType incentiveType)
    {
        var calculator = _calculators.FirstOrDefault(c => c.SupportedIncentiveType == incentiveType);

        if (calculator == null)
        {
            throw new NotSupportedException($"Incentive type {incentiveType} is not supported.");
        }

        return calculator;
    }
}
