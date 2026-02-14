using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Calculators;

public interface IRebateCalculatorFactory
{
    IRebateCalculator GetCalculator(IncentiveType incentiveType);
}
