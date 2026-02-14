using System;
using Smartwyre.DeveloperTest.Application.DTOs;
using Smartwyre.DeveloperTest.Application.Interfaces;
using Smartwyre.DeveloperTest.Domain.Interfaces;

namespace Smartwyre.DeveloperTest.Application.UseCases;

/// <summary>
/// Use Case: Calculate Rebate
/// Implements the core business logic for calculating rebates
/// Clean Architecture - Application layer orchestrates domain logic
/// </summary>
public class CalculateRebateUseCase(
    IRebateRepository rebateRepository,
    IProductRepository productRepository,
    IRebateCalculationStrategyFactory strategyFactory) : ICalculateRebateUseCase
{
    private readonly IRebateRepository _rebateRepository = rebateRepository ?? throw new ArgumentNullException(nameof(rebateRepository));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly IRebateCalculationStrategyFactory _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));

    public CalculateRebateResult Execute(CalculateRebateRequest request)
    {
        var rebate = _rebateRepository.GetById(request.RebateIdentifier);
        if (rebate == null)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"Rebate '{request.RebateIdentifier}' not found."
            };
        }

        var product = _productRepository.GetById(request.ProductIdentifier);
        if (product == null)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"Product '{request.ProductIdentifier}' not found."
            };
        }

        try
        {
            var strategy = _strategyFactory.GetStrategy(rebate.Incentive);

            if (!strategy.CanCalculate(rebate, product, request))
            {
                return new CalculateRebateResult 
                { 
                    Success = false,
                    ErrorMessage = "Rebate calculation validation failed. Check rebate and product compatibility."
                };
            }

            var calculatedAmount = strategy.Calculate(rebate, product, request);
            _rebateRepository.SaveCalculationResult(rebate, calculatedAmount);

            return new CalculateRebateResult
            {
                Success = true,
                CalculatedAmount = calculatedAmount
            };
        }
        catch (NotSupportedException ex)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"Incentive type not supported: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new CalculateRebateResult 
            { 
                Success = false,
                ErrorMessage = $"An error occurred: {ex.Message}"
            };
        }
    }
}