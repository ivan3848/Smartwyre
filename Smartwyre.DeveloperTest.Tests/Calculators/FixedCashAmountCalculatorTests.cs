using Moq;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Calculators;

public class FixedCashAmountCalculatorTests
{
    private readonly FixedCashAmountCalculator _calculator;

    public FixedCashAmountCalculatorTests()
    {
        _calculator = new FixedCashAmountCalculator();
    }

    [Fact]
    public void SupportedIncentiveType_ReturnsFixedCashAmount()
    {
        Assert.Equal(IncentiveType.FixedCashAmount, _calculator.SupportedIncentiveType);
    }

    [Fact]
    public void CanCalculate_WhenRebateIsNull_ReturnsFalse()
    {
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(null, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenProductIsNull_ReturnsFalse()
    {
        var rebate = CreateValidRebate();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, null, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenProductDoesNotSupportIncentive_ReturnsFalse()
    {
        var rebate = CreateValidRebate();
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenRebateAmountIsZero_ReturnsFalse()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 0m
        };
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.False(result);
    }

    [Fact]
    public void CanCalculate_WhenAllConditionsAreMet_ReturnsTrue()
    {
        var rebate = CreateValidRebate();
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.CanCalculate(rebate, product, request);

        Assert.True(result);
    }

    [Fact]
    public void Calculate_ReturnsRebateAmount()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50m
        };
        var product = CreateValidProduct();
        var request = CreateValidRequest();

        var result = _calculator.Calculate(rebate, product, request);

        Assert.Equal(50m, result);
    }

    private Rebate CreateValidRebate()
    {
        return new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50m,
            Percentage = 0.1m
        };
    }

    private Product CreateValidProduct()
    {
        return new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
    }

    private CalculateRebateRequest CreateValidRequest()
    {
        return new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };
    }
}
