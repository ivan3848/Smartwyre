using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class RebateCalculatorFactoryTests
{
    [Fact]
    public void GetCalculator_WithFixedCashAmount_ReturnsCorrectCalculator()
    {
        var calculators = CreateCalculators();
        var factory = new RebateCalculatorFactory(calculators);

        var calculator = factory.GetCalculator(IncentiveType.FixedCashAmount);

        Assert.NotNull(calculator);
        Assert.IsType<FixedCashAmountCalculator>(calculator);
        Assert.Equal(IncentiveType.FixedCashAmount, calculator.SupportedIncentiveType);
    }

    [Fact]
    public void GetCalculator_WithFixedRateRebate_ReturnsCorrectCalculator()
    {
        var calculators = CreateCalculators();
        var factory = new RebateCalculatorFactory(calculators);

        var calculator = factory.GetCalculator(IncentiveType.FixedRateRebate);

        Assert.NotNull(calculator);
        Assert.IsType<FixedRateRebateCalculator>(calculator);
        Assert.Equal(IncentiveType.FixedRateRebate, calculator.SupportedIncentiveType);
    }

    [Fact]
    public void GetCalculator_WithAmountPerUom_ReturnsCorrectCalculator()
    {
        var calculators = CreateCalculators();
        var factory = new RebateCalculatorFactory(calculators);

        var calculator = factory.GetCalculator(IncentiveType.AmountPerUom);

        Assert.NotNull(calculator);
        Assert.IsType<AmountPerUomCalculator>(calculator);
        Assert.Equal(IncentiveType.AmountPerUom, calculator.SupportedIncentiveType);
    }

    [Fact]
    public void Constructor_WithNullCalculators_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RebateCalculatorFactory(null));
    }

    [Fact]
    public void GetCalculator_WithUnsupportedIncentiveType_ThrowsNotSupportedException()
    {
        var calculators = new List<IRebateCalculator>();
        var factory = new RebateCalculatorFactory(calculators);

        Assert.Throws<NotSupportedException>(() => 
            factory.GetCalculator(IncentiveType.FixedCashAmount));
    }

    private IEnumerable<IRebateCalculator> CreateCalculators()
    {
        return new List<IRebateCalculator>
        {
            new FixedCashAmountCalculator(),
            new FixedRateRebateCalculator(),
            new AmountPerUomCalculator()
        };
    }
}
