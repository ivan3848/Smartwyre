# 🧪 Testing Guide with Mock Data

## Overview

The application now includes comprehensive mock data to facilitate testing and demonstration. This guide shows you how to test different rebate calculation scenarios.

## Available Mock Data

### Products

| Product ID | Price | UOM | Supported Incentives |
|-----------|-------|-----|---------------------|
| **PROD001** | $100.00 | kg | FixedCashAmount, FixedRateRebate, AmountPerUom (All) |
| **PROD002** | $250.00 | units | FixedRateRebate |
| **PROD003** | $50.00 | liters | AmountPerUom |
| **PROD004** | $500.00 | units | FixedCashAmount |
| **LAPTOP-X1** | $1,200.00 | units | FixedCashAmount, FixedRateRebate |

### Rebates

| Rebate ID | Type | Amount | Percentage |
|-----------|------|--------|------------|
| **REB001** | FixedCashAmount | $50.00 | - |
| **REB002** | FixedRateRebate | - | 10% |
| **REB003** | AmountPerUom | $5.00 | - |
| **REB004** | FixedRateRebate | - | 15% |
| **REB005** | FixedCashAmount | $100.00 | - |
| **SUMMER2024** | FixedRateRebate | - | 20% |
| **BULK-DISCOUNT** | AmountPerUom | $2.50 | - |

## Quick Start Commands

### View Available Data

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- --list
# or
dotnet run --project Smartwyre.DeveloperTest.Runner -- -l
```

### Get Help

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- --help
# or
dotnet run --project Smartwyre.DeveloperTest.Runner -- -h
```

## Test Scenarios

### 1. Fixed Cash Amount ✅

**Scenario**: Apply a $50 fixed discount

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB001 PROD001 10
```

**Expected Result**:
- ✅ Success
- Amount: $50.00
- Explanation: Rebate gives a fixed $50 regardless of volume

**Alternative Test**:
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB005 LAPTOP-X1 3
```
- Expected: $100.00 (fixed amount)

---

### 2. Fixed Rate Rebate (Percentage) ✅

**Scenario**: Apply a 10% discount on total purchase value

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB002 PROD001 10
```

**Expected Result**:
- ✅ Success
- Amount: $100.00
- Calculation: $100 (price) × 10% × 10 (volume) = $100

**More Examples**:

**15% Discount on expensive product**:
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB004 PROD002 5
```
- Expected: $187.50 ($250 × 15% × 5)

**Summer Sale - 20% off laptops**:
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- SUMMER2024 LAPTOP-X1 2
```
- Expected: $480.00 ($1,200 × 20% × 2)

---

### 3. Amount Per Unit of Measure ✅

**Scenario**: $5 discount per unit

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB003 PROD003 20
```

**Expected Result**:
- ✅ Success
- Amount: $100.00
- Calculation: $5 (per unit) × 20 (volume) = $100

**Bulk Discount Example**:
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- BULK-DISCOUNT PROD001 50
```
- Expected: $125.00 ($2.50 × 50 units)

---

## Error Scenarios (Should Fail)

### Incompatible Product and Rebate

**Test**: Try to apply FixedCashAmount rebate to a product that doesn't support it

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB001 PROD002 10
```

**Expected Result**:
- ❌ Failed
- Error: "Rebate calculation validation failed"
- Reason: PROD002 only supports FixedRateRebate, not FixedCashAmount

---

### Non-existent Rebate

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- INVALID REB001 10
```

**Expected Result**:
- ❌ Failed
- Error: "Rebate 'INVALID' not found."

---

### Non-existent Product

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB001 INVALID 10
```

**Expected Result**:
- ❌ Failed
- Error: "Product 'INVALID' not found."

---

### Zero Volume

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB002 PROD001 0
```

**Expected Result**:
- ❌ Failed
- Error: "Rebate calculation validation failed"
- Reason: Volume must be greater than 0 for FixedRateRebate

---

## Interactive Mode

Run without arguments to use interactive mode:

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner
```

The application will prompt you for:
1. Rebate Identifier
2. Product Identifier
3. Volume

**Example Session**:
```
Enter Rebate Identifier (e.g., REB001, SUMMER2024): SUMMER2024
Enter Product Identifier (e.g., PROD001, LAPTOP-X1): LAPTOP-X1
Enter Volume: 2

═══════════════════════════════════════
✅ Rebate calculation SUCCEEDED!

💰 Calculated Rebate Amount: $480.00
📊 Calculation Details:
   - Rebate: SUMMER2024
   - Product: LAPTOP-X1
   - Volume: 2

💾 Result has been saved to the database.
═══════════════════════════════════════
```

---

## Calculation Formulas

### Fixed Cash Amount
```
Rebate Amount = Fixed Amount
```
**Example**: REB001 always returns $50.00

### Fixed Rate Rebate
```
Rebate Amount = Product Price × Percentage × Volume
```
**Example**: $100 × 10% × 10 = $100.00

### Amount Per UOM
```
Rebate Amount = Amount Per Unit × Volume
```
**Example**: $5 × 20 = $100.00

---

## Testing Checklist

Use this checklist to verify all functionality:

- [ ] **Fixed Cash Amount**
  - [ ] REB001 + PROD001 + Volume 10 → $50.00 ✅
  - [ ] REB005 + LAPTOP-X1 + Volume 5 → $100.00 ✅

- [ ] **Fixed Rate Rebate**
  - [ ] REB002 + PROD001 + Volume 10 → $100.00 ✅
  - [ ] SUMMER2024 + LAPTOP-X1 + Volume 2 → $480.00 ✅
  - [ ] REB004 + PROD002 + Volume 5 → $187.50 ✅

- [ ] **Amount Per UOM**
  - [ ] REB003 + PROD003 + Volume 20 → $100.00 ✅
  - [ ] BULK-DISCOUNT + PROD001 + Volume 50 → $125.00 ✅

- [ ] **Error Handling**
  - [ ] Invalid rebate ID → Error ❌
  - [ ] Invalid product ID → Error ❌
  - [ ] Incompatible product/rebate → Error ❌
  - [ ] Zero volume → Error ❌

---

## Advanced Scenarios

### High Volume Purchase

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- BULK-DISCOUNT PROD001 1000
```
- Expected: $2,500.00 (great bulk savings!)

### Expensive Product with Discount

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB004 PROD004 10
```
- Expected: $750.00 ($500 × 15% × 10)

### Minimum Volume Test

```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB003 PROD003 1
```
- Expected: $5.00 (minimum viable calculation)

---

## Tips for Testing

1. **Use --list first**: See all available data before testing
2. **Check compatibility**: Ensure product supports the rebate type
3. **Verify calculations**: Use the formulas above to confirm results
4. **Test edge cases**: Zero values, very large values, etc.
5. **Test error paths**: Invalid IDs, incompatible combinations

---

## Next Steps

After testing with mock data, you can:

1. **Add new products**: Update `MockDataStore.cs`
2. **Add new rebates**: Update `MockDataStore.cs`
3. **Implement real database**: Replace mock repositories with actual DB access
4. **Add logging**: Track all calculations
5. **Add reporting**: View calculation history

---

**Happy Testing!** 🎉

For questions or issues, refer to:
- `ARCHITECTURE.md` - Architecture details
- `SOLUTION_README.md` - Implementation guide
- `CLEAN_ARCHITECTURE_MIGRATION.md` - Clean Architecture details
