using System;
using Moq;
using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Application.UseCases;
using Smartwyre.DeveloperTest.Domain.Entities;
using Smartwyre.DeveloperTest.Domain.Enums;
using Smartwyre.DeveloperTest.Domain.Interfaces;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests.Application.UseCases;

/// <summary>
/// Unit tests for CalculateRebateUseCase
/// Testing Application layer - Clean Architecture
/// </summary>
public class CalculateRebateUseCaseTests
{
    private readonly Mock<IRebateRepository> _mockRebateRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IRebateCalculationStrategyFactory> _mockStrategyFactory;
    private readonly CalculateRebateUseCase _useCase;

    public CalculateRebateUseCaseTests()
    {
        _mockRebateRepository = new Mock<IRebateRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockStrategyFactory = new Mock<IRebateCalculationStrategyFactory>();
        
        _useCase = new CalculateRebateUseCase(
            _mockRebateRepository.Object,
            _mockProductRepository.Object,
            _mockStrategyFactory.Object);
    }

    [Fact]
    public void Constructor_WithNullRebateRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new CalculateRebateUseCase(null, _mockProductRepository.Object, _mockStrategyFactory.Object));
    }

    [Fact]
    public void Execute_WhenRebateNotFound_ReturnsFailureWithMessage()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "UNKNOWN",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        _mockRebateRepository.Setup(x => x.GetById("UNKNOWN")).Returns((Rebate)null);

        var result = _useCase.Execute(request);

        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessage);
        _mockRebateRepository.Verify(x => x.SaveCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Execute_WhenProductNotFound_ReturnsFailureWithMessage()
    {
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "UNKNOWN",
            Volume = 10m
        };

        var rebate = new Rebate { Identifier = "REB001" };
        _mockRebateRepository.Setup(x => x.GetById("REB001")).Returns(rebate);
        _mockProductRepository.Setup(x => x.GetById("UNKNOWN")).Returns((Product)null);

        var result = _useCase.Execute(request);

        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessage);
    }

    [Fact]
    public void Execute_WhenValidationFails_ReturnsFailureWithMessage()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 0m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        var mockStrategy = new Mock<IRebateCalculationStrategy>();
        mockStrategy.Setup(x => x.CanCalculate(rebate, product, request)).Returns(false);

        _mockRebateRepository.Setup(x => x.GetById("REB001")).Returns(rebate);
        _mockProductRepository.Setup(x => x.GetById("PROD001")).Returns(product);
        _mockStrategyFactory.Setup(x => x.GetStrategy(IncentiveType.FixedCashAmount))
            .Returns(mockStrategy.Object);

        var result = _useCase.Execute(request);

        Assert.False(result.Success);
        Assert.Contains("validation failed", result.ErrorMessage);
    }

    [Fact]
    public void Execute_WhenSuccessful_ReturnsSuccessWithAmount()
    {
        var rebate = new Rebate
        {
            Identifier = "REB001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            Price = 100m,
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REB001",
            ProductIdentifier = "PROD001",
            Volume = 10m
        };

        var mockStrategy = new Mock<IRebateCalculationStrategy>();
        mockStrategy.Setup(x => x.CanCalculate(rebate, product, request)).Returns(true);
        mockStrategy.Setup(x => x.Calculate(rebate, product, request)).Returns(50m);

        _mockRebateRepository.Setup(x => x.GetById("REB001")).Returns(rebate);
        _mockProductRepository.Setup(x => x.GetById("PROD001")).Returns(product);
        _mockStrategyFactory.Setup(x => x.GetStrategy(IncentiveType.FixedCashAmount))
            .Returns(mockStrategy.Object);

        var result = _useCase.Execute(request);

        Assert.True(result.Success);
        Assert.Equal(50m, result.CalculatedAmount);
        _mockRebateRepository.Verify(x => x.SaveCalculationResult(rebate, 50m), Times.Once);
    }
}
