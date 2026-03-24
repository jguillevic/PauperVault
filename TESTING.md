# 🧪 Guide de Test

Guide complet pour les tests dans **PauperVault**.

## 📋 Table des matières

1. [Vue d'ensemble](#vue-densemble)
2. [Tests unitaires](#tests-unitaires)
3. [Tests d'intégration](#tests-dintégration)
4. [Tests de performance](#tests-de-performance)
5. [Tests de sécurité](#tests-de-sécurité)
6. [Couverture de code](#couverture-de-code)
7. [CI/CD](#cicd)

## 🎯 Vue d'ensemble

### Stratégie de test

```
┌──────────────────────────────────────────┐
│      Pyramide de test (Test Pyramid)     │
├──────────────────────────────────────────┤
│           E2E / Smoke Tests   │ 5%       │
│          (Rare, précieux)     │          │
├──────────────────────────────────────────┤
│     Integration Tests         │ 15%      │
│     (Avec BD, services)       │          │
├──────────────────────────────────────────┤
│     Unit Tests                │ 80%      │
│     (Rapides, isolés)         │          │
└──────────────────────────────────────────┘
```

### Types de tests

| Type | Portée | Vitesse | Coût | % |
|------|--------|--------|------|---|
| Unit | Isolé | ⚡⚡⚡ | $ | 80 |
| Integration | Services | ⚡⚡ | $$ | 15 |
| E2E | Complet | ⚡ | $$$ | 5 |

## ✅ Tests unitaires

### Structure du projet

```
PauperVault.Core.Tests/
├─ Domain/
│  ├─ Cards/
│  │  ├─ CardRulesTests.cs
│  │  └─ CardValidationTests.cs
│  │
│  └─ Decks/
│     ├─ DeckRulesTests.cs
│     ├─ DeckValidationTests.cs
│     └─ DeckStatisticsTests.cs
│
└─ Services/
   ├─ CardServiceTests.cs
   └─ DeckServiceTests.cs
```

### Exemple de test unitaire

```csharp
using NUnit.Framework;
using Moq;

[TestFixture]
public class DeckRulesTests
{
    private DeckRules _deckRules;
    
    [SetUp]
    public void Setup()
    {
        _deckRules = new DeckRules();
    }
    
    [Test]
    public void ValidateDeckSize_WithLessThan60Cards_ThrowsException()
    {
        // Arrange
        var deck = CreateDeckWithCardCount(59);
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidDeckException>(
            () => _deckRules.ValidateAsync(deck)
        );
        
        Assert.That(exception.Message, Contains.Substring("60 cards required"));
    }
    
    [Test]
    public void ValidateDeckSize_WithExactly60Cards_DoesNotThrow()
    {
        // Arrange
        var deck = CreateDeckWithCardCount(60);
        
        // Act & Assert
        Assert.DoesNotThrowAsync(() => _deckRules.ValidateAsync(deck));
    }
    
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public void ValidateCardLimit_WithExceedingLimit_ThrowsException(int quantity)
    {
        // Arrange
        var card = new Card { Name = "Test", Rarity = CardRarity.Common };
        var deck = new Deck();
        
        // Act & Assert (should fail for qty > 4)
        if (quantity > 4)
        {
            Assert.ThrowsAsync<DeckRulesViolationException>(
                () => _deckRules.ValidateCardQuantityAsync(card, quantity)
            );
        }
        else
        {
            Assert.DoesNotThrowAsync(
                () => _deckRules.ValidateCardQuantityAsync(card, quantity)
            );
        }
    }
    
    // Helper method
    private Deck CreateDeckWithCardCount(int count)
    {
        var deck = new Deck { Name = "Test Deck" };
        for (int i = 0; i < count; i++)
        {
            deck.Cards.Add(new DeckCard 
            { 
                Card = new Card { Id = i, Name = $"Card{i}" },
                Quantity = 1
            });
        }
        return deck;
    }
}
```

### Patterns de test

#### AAA Pattern (Arrange-Act-Assert)

```csharp
[Test]
public void ProcessDeck_WithValidData_ReturnsSuccess()
{
    // Arrange - Préparer les données
    var deck = new Deck { Name = "Test", Cards = new List<DeckCard>() };
    var service = new DeckService();
    
    // Act - Exécuter l'action
    var result = service.Process(deck);
    
    // Assert - Vérifier le résultat
    Assert.That(result.IsSuccess, Is.True);
}
```

#### Given-When-Then Pattern

```csharp
[Test]
public void GivenAValidDeck_WhenProcessing_ThenReturnSuccess()
{
    // Given - État initial
    var deck = new Deck { Name = "Test" };
    
    // When - Action
    var result = new DeckService().Process(deck);
    
    // Then - Résultat attendu
    Assert.That(result.IsSuccess, Is.True);
}
```

### Mocking avec Moq

```csharp
[Test]
public async Task AddCardToDeck_WithValidCard_CallsRepository()
{
    // Arrange
    var mockRepository = new Mock<IDeckRepository>();
    var mockCard = new Mock<ICard>();
    
    mockRepository
        .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        .ReturnsAsync(new Deck { Id = 1 });
    
    var service = new DeckService(mockRepository.Object);
    
    // Act
    await service.AddCardAsync(1, mockCard.Object);
    
    // Assert
    mockRepository.Verify(
        r => r.UpdateAsync(It.IsAny<Deck>()),
        Times.Once
    );
}
```

### Bonnes pratiques

```csharp
// ✅ BON
[Test]
public void Validate_WithNullInput_ThrowsArgumentNullException()
{
    Assert.Throws<ArgumentNullException>(() => _rules.Validate(null));
}

// ❌ MAUVAIS
[Test]
public void Test1() // Nom peu clair
{
    Assert.That(true, Is.True); // Test trivial
}

// ❌ MAUVAIS
[Test]
public void TestEverything() // Teste trop de choses
{
    var deck = new Deck();
    deck.AddCard(...);
    deck.RemoveCard(...);
    deck.Validate();
    Assert.That(...);
}
```

## 🔗 Tests d'intégration

### Structure

```
PauperVault.Api.Tests/
├─ Features/
│  ├─ Decks/
│  │  ├─ CreateDeckHandlerTests.cs
│  │  ├─ UpdateDeckHandlerTests.cs
│  │  └─ GetDeckHandlerTests.cs
│  │
│  └─ Cards/
│     ├─ GetCardsHandlerTests.cs
│     └─ SearchCardsHandlerTests.cs
│
└─ Infrastructure/
   ├─ PersistenceTests.cs
   └─ AuthenticationTests.cs
```

### Exemple de test d'intégration

```csharp
using Xunit;

[Collection("Database Collection")]
public class CreateDeckHandlerIntegrationTests : IAsyncLifetime
{
    private readonly PauperVaultDbContext _context;
    private readonly CreateDeckCommandHandler _handler;
    private readonly IMapper _mapper;
    
    public CreateDeckHandlerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<PauperVaultDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        _context = new PauperVaultDbContext(options);
        var repository = new DeckRepository(_context);
        _handler = new CreateDeckCommandHandler(repository, _mapper);
    }
    
    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task Handle_WithValidCommand_CreatesDecks()
    {
        // Arrange
        var command = new CreateDeckCommand("Test Deck");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Deck", result.Name);
        Assert.NotEqual(0, result.Id);
    }
    
    [Fact]
    public async Task Handle_WithDuplicateName_ThrowsException()
    {
        // Arrange
        var command1 = new CreateDeckCommand("Test Deck");
        var command2 = new CreateDeckCommand("Test Deck");
        
        await _handler.Handle(command1, CancellationToken.None);
        
        // Act & Assert
        await Assert.ThrowsAsync<DuplicateDeckException>(
            () => _handler.Handle(command2, CancellationToken.None)
        );
    }
}
```

### Fixtures réutilisables

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PauperVaultDbContext _context;
    
    public async Task InitializeAsync()
    {
        // Setup: créer la BD
        var options = new DbContextOptionsBuilder<PauperVaultDbContext>()
            .UseSqlServer("Server=.;Database=TestDb;Integrated Security=true;")
            .Options;
        
        _context = new PauperVaultDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }
    
    public async Task DisposeAsync()
    {
        // Cleanup
        await _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }
}

[Collection("Database Collection")]
public class DeckRepositoryTests
{
    private readonly DatabaseFixture _fixture;
    
    public DeckRepositoryTests(DatabaseFixture fixture) => _fixture = fixture;
    
    [Fact]
    public async Task GetById_WithValidId_ReturnsDeck()
    {
        // Test avec la fixture
    }
}
```

## ⚡ Tests de performance

### Benchmark

```csharp
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class DeckSearchBenchmark
{
    private PauperVaultDbContext _context;
    private DeckRepository _repository;
    private List<Deck> _decks;
    
    [GlobalSetup]
    public void Setup()
    {
        // Créer 10K decks
        _context = CreateContext();
        _decks = GenerateDecks(10000);
        _context.Decks.AddRange(_decks);
        _context.SaveChanges();
    }
    
    [Benchmark]
    public async Task SearchDecksByName()
    {
        await _repository.SearchAsync(d => d.Name.Contains("Test"));
    }
    
    [Benchmark]
    public async Task SearchWithInclude()
    {
        await _repository.SearchAsync(
            d => d.Name.Contains("Test"),
            d => d.Include(x => x.Cards)
        );
    }
}

// Exécuter
// dotnet run -c Release --project PauperVault.Performance.Tests
```

### Tests de charge

```bash
# Utiliser Apache JMeter ou k6

# Script k6 (JavaScript)
import http from 'k6/http';
import { check } from 'k6';

export let options = {
  vus: 100,              // 100 utilisateurs virtuels
  duration: '30s',       // 30 secondes
  rps: 1000,             // 1000 requêtes/sec max
};

export default function() {
  let response = http.get('https://paupervault.com/api/decks');
  
  check(response, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
}
```

## 🔒 Tests de sécurité

### Validation des inputs

```csharp
[TestFixture]
public class SecurityTests
{
    [TestCase("<script>alert('xss')</script>")]
    [TestCase("'; DROP TABLE Decks; --")]
    [TestCase("../../../etc/passwd")]
    public void CreateDeck_WithMaliciousInput_Sanitizes(string input)
    {
        var command = new CreateDeckCommand(input);
        
        // Le service doit sanitizer l'entrée
        Assert.DoesNotThrow(() => _service.Create(command));
    }
    
    [Test]
    public void UpdateCard_WithoutAuthorization_ThrowsUnauthorizedException()
    {
        // Arrangement
        var unauthorizedUser = new User { Id = 999 };
        var command = new UpdateCardCommand(1, "New Name");
        
        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedException>(
            () => _cardService.UpdateAsync(command, unauthorizedUser)
        );
    }
}
```

## 📊 Couverture de code

### Mesurer la couverture

```bash
# Générer le rapport de couverture
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Visualiser avec ReportGenerator
reportgenerator -reports:"**/coverage.opencover.xml" \
                -targetdir:"CoverageReport" \
                -reporttypes:Html
```

### Cibles de couverture

| Component | Cible | Status |
|-----------|-------|--------|
| Domain (Core) | > 90% | ✅ |
| Handlers | > 80% | ✅ |
| Controllers | > 70% | ⚠️ |
| UI (Razor Pages) | > 60% | ⚠️ |
| Infrastructure | > 70% | ✅ |
| **Global** | **> 80%** | ✅ |

## 🔄 CI/CD

### GitHub Actions - Test Pipeline

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      sql-server:
        image: mcr.microsoft.com/mssql/server:latest
        env:
          ACCEPT_EULA: Y
          SA_PASSWORD: Test@1234
        options: >-
          --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Test@1234 -Q \"SELECT 1\""
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 1433:1433
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore -c Release
    
    - name: Unit Tests
      run: dotnet test --filter "Category!=Integration" --no-build
    
    - name: Integration Tests
      run: dotnet test --filter "Category=Integration" --no-build
    
    - name: Code Coverage
      run: dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
    
    - name: Upload Coverage to Codecov
      uses: codecov/codecov-action@v3
```

## 📝 Checklist de test avant le merge

- [ ] Tous les tests unitaires passent
- [ ] Tests d'intégration passent
- [ ] Pas de regressions détectées
- [ ] Couverture de code > 80%
- [ ] Pas de violations de sécurité
- [ ] Performance acceptable
- [ ] Code review approuvée

---

**Dernière mise à jour :** 2026
**Version :** 1.0  
**Responsable :** QA Team
