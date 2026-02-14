using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public class RebateCalculatorFactory(IEnumerable<IRebateCalculator> calculators) : IRebateCalculatorFactory
{
    private readonly IEnumerable<IRebateCalculator> _calculators = calculators ?? throw new ArgumentNullException(nameof(calculators));

    public IRebateCalculator GetCalculator(IncentiveType incentiveType)
    {
        var calculator = _calculators.FirstOrDefault(c => c.SupportedIncentiveType == incentiveType);
        return calculator ?? throw new NotSupportedException($"Incentive type {incentiveType} is not supported.");
    }
}
