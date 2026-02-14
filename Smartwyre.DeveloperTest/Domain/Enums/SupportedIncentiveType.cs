using System;

namespace Smartwyre.DeveloperTest.Domain.Enums;

/// <summary>
/// Supported incentive types that can be combined using bitwise operations
/// </summary>
[Flags]
public enum SupportedIncentiveType
{
    FixedRateRebate = 1 << 0,
    AmountPerUom = 1 << 1,
    FixedCashAmount = 1 << 2,
}
