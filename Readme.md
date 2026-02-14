# Smartwyre Developer Test Instructions

You have been selected to complete our candidate coding exercise. Please follow the directions in this readme.

Clone, **DO NOT FORK**, this repository to your account on the online Git resource of your choosing (GitHub, BitBucket, GitLab, etc.). Your solution should retain previous commit history and you should utilize best practices for committing your changes to the repository.

You are welcome to use whatever tools you normally would when coding — including documentation, libraries, frameworks, or AI tools (such as ChatGPT or Copilot).

However, it is important that you fully understand your solution. As part of the interview process, we will review your code with you in detail. You should be able to:

- Explain the design choices you made.
- Walk us through how your solution works.
- Make modifications or extensions to your code during the review.

Please note: if your submission appears to have been generated entirely by an AI agent or another third party, without your own understanding or contribution, it will not meet our evaluation criteria.

# The Exercise

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - Currently there are 3 known incentive types. In the future the business will want to add many more incentive types. Your solution should make it easy for developers to add new incentive types in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs (either via command line arguments or via prompts is fine)

The only specific "rules" are:

- The solution must build
- All tests must pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.

Feel free to use code comments to describe your changes. You are also welcome to update this readme with any important details for us to consider.

Once you have completed the exercise either ensure your repository is available publicly or contact the hiring manager to set up a private share.

---

# Solution Summary

## ✅ Completed Requirements

- ✅ **SOLID Principles**: Full implementation with Strategy, Factory, and Dependency Injection patterns
- ✅ **Testability**: 50 comprehensive unit tests (all passing)
- ✅ **Readability**: Clean code with clear separation of concerns
- ✅ **Extensibility**: Easy to add new incentive types without modifying existing code
- ✅ **Console Application**: Supports both command-line arguments and interactive prompts
- ✅ **Solution Builds**: Zero errors, zero warnings
- ✅ **All Tests Pass**: 50/50 tests passing

## 🎯 Key Design Decisions

### 1. Clean Architecture (Onion/Hexagonal Architecture)
The solution follows **Clean Architecture** principles with clear layer separation:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  ┌───────────────────────────────────┐  │
│  │     Application Layer             │  │
│  │  ┌─────────────────────────────┐  │  │
│  │  │    Domain Layer (Core)      │  │  │
│  │  │  - Zero Dependencies        │  │  │
│  │  │  - Pure Business Logic      │  │  │
│  │  └─────────────────────────────┘  │  │
│  │  Use Cases & Strategies           │  │
│  └───────────────────────────────────┘  │
│  Infrastructure (Repositories)          │
└─────────────────────────────────────────┘
```

**Layers**:
- **Domain** (Core): Entities, Enums, Repository Interfaces - Zero dependencies
- **Application**: Use Cases, Strategies, DTOs - Depends only on Domain
- **Infrastructure**: Repository implementations - Implements Domain interfaces
- **Presentation**: Console UI, DI configuration - Depends on all layers

**Benefits**:
- ✅ Framework-independent business logic
- ✅ Database-independent (Repository Pattern)
- ✅ UI-independent (Use Case Pattern)
- ✅ Highly testable (50 unit tests)

### 2. Strategy Pattern for Calculators
Each incentive type has its own strategy class implementing `IRebateCalculationStrategy`:
- `FixedCashAmountStrategy`
- `FixedRateRebateStrategy`
- `AmountPerUomStrategy`

**Benefit**: Adding new incentive types requires only creating a new strategy class—no modifications to existing code (Open/Closed Principle).

### 3. Use Case Pattern
Business operations encapsulated as use cases:
- `CalculateRebateUseCase` - Orchestrates rebate calculation

**Benefit**: Clear separation between business logic and infrastructure.

### 4. Repository Pattern
Data access abstracted behind interfaces:
- `IRebateRepository` / `RebateRepository`
- `IProductRepository` / `ProductRepository`

**Benefit**: Easy to swap database implementations without touching business logic.

### 5. Dependency Injection
All dependencies injected via constructors using `Microsoft.Extensions.DependencyInjection`:
- Domain interfaces → Infrastructure implementations
- Application use cases → Injected into Presentation

**Benefit**: Loose coupling and easy mocking for unit tests.

## 🚀 How to Run

### Build
```bash
dotnet build
```

### Run Tests (50 tests)
```bash
dotnet test
```

### Run Console Application

#### View Available Mock Data
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- --list
```

This will show all available products and rebates with their details.

#### Calculate a Rebate

**Option 1: Command-line arguments**
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- <rebate-id> <product-id> <volume>
```

**Examples**:
```bash
# Fixed Cash Amount - $50 rebate
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB001 PROD001 10

# Fixed Rate Rebate - 20% off laptops
dotnet run --project Smartwyre.DeveloperTest.Runner -- SUMMER2024 LAPTOP-X1 2

# Amount Per UOM - $5 per unit
dotnet run --project Smartwyre.DeveloperTest.Runner -- REB003 PROD003 20
```

**Option 2: Interactive prompts**
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner
```
Then enter: Rebate ID, Product ID, and Volume when prompted.

**Option 3: Get help**
```bash
dotnet run --project Smartwyre.DeveloperTest.Runner -- --help
```

## 📊 Test Coverage

- **Use Case Tests**: 5 tests for business logic orchestration
- **Strategy Tests**: 31 tests covering all calculation strategies
- **Factory Tests**: 5 tests for strategy selection logic
- **Service Tests**: 9 integration tests (legacy)

Total: **50 tests** - All tests use **xUnit** and **Moq** frameworks.

## 📚 Additional Documentation

- **SOLUTION_README.md** - Detailed implementation guide
- **ARCHITECTURE.md** - Architecture overview and SOLID principles explanation
- **REFACTORING_SUMMARY.md** - Before/after comparison

## 🔄 Adding New Incentive Types

1. Create a new calculator class implementing `IRebateCalculator`
2. Register it in DI configuration (`Program.cs`)
3. Write unit tests

**No changes to existing classes required!**

Example:
```csharp
public class NewIncentiveCalculator : IRebateCalculator
{
    public IncentiveType SupportedIncentiveType => IncentiveType.NewType;
    public bool CanCalculate(...) { /* validation */ }
    public decimal Calculate(...) { /* calculation */ }
}
```

## 🛠️ Technologies Used

- .NET 10
- xUnit 2.9.3
- Moq 4.20.72
- Microsoft.Extensions.DependencyInjection 9.0.0

---
