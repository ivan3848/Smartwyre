using Smartwyre.DeveloperTest.Application.DTOs;

namespace Smartwyre.DeveloperTest.Application.Interfaces;

/// <summary>
/// Use case interface for calculating rebates
/// Part of Application layer - defines business operation
/// </summary>
public interface ICalculateRebateUseCase
{
    CalculateRebateResult Execute(CalculateRebateRequest request);
}
