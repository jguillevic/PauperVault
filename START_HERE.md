# 📖 Guide de Démarrage pour Développeurs

Bienvenue dans **PauperVault**! Ce guide vous aide à démarrer en tant que développeur.

## 🎯 Avant de commencer

### Vous devriez savoir

- C# basique (variables, classes, interfaces)
- ASP.NET Core (controllers, Razor Pages)
- Entity Framework Core
- LINQ
- Git & GitHub

### Pas familier? Consultez

- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [ASP.NET Core Docs](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## 🚀 Démarrage rapide (5 minutes)

```bash
# 1. Cloner le repo
git clone https://github.com/jguillevic/PauperVault.git
cd PauperVault

# 2. Restaurer les dépendances
dotnet restore

# 3. Configurer les secrets
dotnet user-secrets init --project PauperVault.Api
dotnet user-secrets set "Jwt:SecretKey" "dev-key-here" --project PauperVault.Api

# 4. Créer la base de données
dotnet ef database update --project PauperVault.Api

# 5. Lancer l'application
dotnet run --project PauperVault.Api

# 6. Accéder à l'application
# https://localhost:7001
```

Pour plus de détails, voir [INSTALLATION.md](INSTALLATION.md).

## 📂 Structure du projet - Explication rapide

```
PauperVault/
│
├─ PauperVault.Core/          ← Logique métier (ce qui change le moins)
│  └─ Domain/                   Entities, Rules, Services
│
├─ PauperVault.Api/            ← Serveur ASP.NET Core
│  ├─ Features/                 CQRS (Commandes & Requêtes)
│  ├─ Controllers/              API Endpoints
│  └─ Infrastructure/           Base de données, Auth, Config
│
├─ PauperVault.Web/            ← Interface utilisateur (Razor Pages)
│  ├─ Pages/                    Pages et handlers
│  ├─ Components/               Composants réutilisables
│  └─ wwwroot/css/              Stylesheets
│
└─ PauperVault.Contracts/      ← DTOs partagés
   ├─ Requests/
   ├─ Responses/
   └─ Common/
```

## 🏗️ Concepts clés

### 1. Domain-Driven Design (DDD)

La logique métier vit dans **PauperVault.Core**.

```csharp
// Exemple: Créer un deck
// Dans PauperVault.Core/Domain/Decks/

public class Deck
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IList<DeckCard> Cards { get; set; }
    
    // Logique métier (règles Pauper)
    public void AddCard(Card card, int quantity)
    {
        if (card.Rarity == CardRarity.Common)
            throw new InvalidOperationException("Non-common cards not allowed in Pauper");
        // ...
    }
}
```

### 2. CQRS (Commandes & Requêtes)

Les modifications utilisent des **Commands**, les lectures utilisent des **Queries**.

```csharp
// Command (écriture) - Créer un deck
public record CreateDeckCommand(string Name) : IRequest<DeckResponse>;

public class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, DeckResponse>
{
    public async Task<DeckResponse> Handle(CreateDeckCommand request, CancellationToken ct)
    {
        var deck = new Deck { Name = request.Name };
        await _deckRepository.AddAsync(deck);
        return _mapper.Map<DeckResponse>(deck);
    }
}

// Query (lecture) - Récupérer un deck
public record GetDeckByIdQuery(int Id) : IRequest<DeckResponse>;

public class GetDeckByIdQueryHandler : IRequestHandler<GetDeckByIdQuery, DeckResponse>
{
    public async Task<DeckResponse> Handle(GetDeckByIdQuery request, CancellationToken ct)
    {
        var deck = await _deckRepository.GetByIdAsync(request.Id);
        return _mapper.Map<DeckResponse>(deck);
    }
}
```

### 3. Dependency Injection

Tout est configuré dans `Program.cs`.

```csharp
// Program.cs
builder.Services.AddScoped<IDeckRepository, DeckRepository>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddMediatR(config => 
    config.RegisterServicesFromAssemblyContaining(typeof(Program)));
```

## 📚 Tâches courantes

### Ajouter une nouvelle colonne à une entité

```csharp
// 1. Modifier l'entity dans PauperVault.Core/Domain/Decks/Deck.cs
public class Deck
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }  // ← Nouvelle colonne
}

// 2. Créer une migration
dotnet ef migrations add AddDescriptionToDeck --project PauperVault.Api

// 3. Appliquer la migration
dotnet ef database update --project PauperVault.Api

// 4. Mettre à jour les DTOs si nécessaire
// PauperVault.Contracts/Responses/DeckResponse.cs
public class DeckResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }  // ← Ajouter ici aussi
}
```

### Créer un nouveau endpoint API

```csharp
// 1. Créer la Query/Command
// PauperVault.Api/Features/Decks/Queries/
public record GetDeckStatisticsQuery(int DeckId) : IRequest<DeckStatisticsResponse>;

// 2. Créer le handler
public class GetDeckStatisticsQueryHandler : IRequestHandler<GetDeckStatisticsQuery, DeckStatisticsResponse>
{
    private readonly IDeckRepository _deckRepository;
    
    public async Task<DeckStatisticsResponse> Handle(GetDeckStatisticsQuery request, CancellationToken ct)
    {
        var deck = await _deckRepository.GetByIdAsync(request.DeckId);
        if (deck == null)
            throw new DeckNotFoundException(request.DeckId);
        
        return new DeckStatisticsResponse
        {
            TotalCards = deck.Cards.Sum(c => c.Quantity),
            AverageMana = CalculateAverageMana(deck),
            // ...
        };
    }
}

// 3. Créer l'endpoint dans le Controller
// PauperVault.Api/Controllers/DecksController.cs
[HttpGet("{id}/statistics")]
public async Task<ActionResult<DeckStatisticsResponse>> GetStatistics(int id)
{
    var query = new GetDeckStatisticsQuery(id);
    var result = await _mediator.Send(query);
    return Ok(result);
}

// 4. Utiliser depuis la Razor Page
// PauperVault.Web/Pages/Decks/View.cshtml.cs
public async Task OnGetAsync(int id)
{
    var response = await _httpClient.GetAsync($"/api/decks/{id}/statistics");
    DeckStatistics = await response.Content.ReadAsAsync<DeckStatisticsResponse>();
}
```

### Ajouter une règle métier

```csharp
// 1. Ajouter dans PauperVault.Core/Domain/Decks/DeckRules.cs
public static class DeckRules
{
    private const int MaxCardsInDeck = 60;
    private const int MaxCardsInSideboard = 15;
    
    public static void ValidateDeckComposition(Deck deck)
    {
        var mainDeckCount = deck.Cards
            .Where(c => c.IsInMainDeck)
            .Sum(c => c.Quantity);
        
        if (mainDeckCount > MaxCardsInDeck)
            throw new InvalidDeckException($"Main deck exceeds {MaxCardsInDeck} cards");
        
        // Ajouter d'autres validations...
    }
}

// 2. Utiliser dans le handler
public class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, DeckResponse>
{
    public async Task<DeckResponse> Handle(CreateDeckCommand request, CancellationToken ct)
    {
        var deck = new Deck { Name = request.Name };
        
        // Appliquer les règles
        DeckRules.ValidateDeckComposition(deck);
        
        await _deckRepository.AddAsync(deck);
        return _mapper.Map<DeckResponse>(deck);
    }
}
```

## 🧪 Tester votre code

### Tests unitaires

```csharp
// PauperVault.Core.Tests/Domain/Decks/DeckRulesTests.cs
[TestFixture]
public class DeckRulesTests
{
    [Test]
    public void ValidateDeckComposition_WithMoreThan60Cards_ThrowsException()
    {
        // Arrange
        var deck = new Deck { Name = "Test" };
        for (int i = 0; i < 61; i++)
        {
            deck.Cards.Add(new DeckCard { Quantity = 1 });
        }
        
        // Act & Assert
        Assert.Throws<InvalidDeckException>(() => 
            DeckRules.ValidateDeckComposition(deck));
    }
}

// Exécuter les tests
dotnet test
```

### Tests avec Swagger

1. Ouvrir https://localhost:7001/swagger
2. Tester les endpoints directement
3. Voir les réponses en temps réel

## 🔍 Debugging

### Breakpoints

```csharp
// Mettre un breakpoint dans le code
public async Task<DeckResponse> Handle(GetDeckByIdQuery request, CancellationToken ct)
{
    var deck = await _deckRepository.GetByIdAsync(request.Id); // ← Clic sur la marge
    return _mapper.Map<DeckResponse>(deck);
}

// F5 pour démarrer le debugging
// F10 pour avancer ligne par ligne
// F11 pour entrer dans une fonction
// Maj+F5 pour arrêter
```

### Logs

```csharp
// Activer les logs
// Dans appsettings.Development.json
"Logging": {
    "LogLevel": {
        "Default": "Debug",
        "Microsoft.EntityFrameworkCore": "Information"
    }
}

// Voir les logs SQL
[Output Tool Window > Build]
```

### Watch window

- Debug > Windows > Watch
- Entrer le nom d'une variable pour la surveiller
- Voir sa valeur en temps réel

## 📝 Fichiers importants à connaître

| Fichier | Responsabilité |
|---------|-----------------|
| `Program.cs` (Api) | Configuration des services |
| `Startup.cs` (Web) | Configuration du web |
| `appsettings.json` | Configuration générale |
| `PauperVaultDbContext.cs` | Modèle de base de données |
| `DeckRules.cs` | Logique métier Decks |
| `CardRules.cs` | Logique métier Cards |

## 🌐 Ressources pour développeurs

### Documentation
- [START_HERE.md](START_HERE.md) - Vous êtes ici
- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture détaillée
- [INSTALLATION.md](INSTALLATION.md) - Setup du projet
- [CONTRIBUTING.md](CONTRIBUTING.md) - Guide de contribution

### Outils essentiels
- **Visual Studio** : IDE principal
- **Git** : Contrôle de version
- **Postman** : Tester les APIs
- **LINQPad** : Tester LINQ

### Lire ensuite

1. **Architecture** : Lire [ARCHITECTURE.md](ARCHITECTURE.md) pour comprendre la structure
2. **Installation** : Suivre [INSTALLATION.md](INSTALLATION.md) pour configurer votre environnement
3. **Contribution** : Lire [CONTRIBUTING.md](CONTRIBUTING.md) avant de modifier le code

## 🤔 FAQ

### Q: Où ajouter le code métier?
**R:** Dans `PauperVault.Core/Domain/`. C'est le cœur de l'application, sans dépendances externes.

### Q: Comment créer un endpoint API?
**R:** 
1. Créer Command/Query dans `Features/`
2. Créer le handler
3. Ajouter l'endpoint dans le Controller
4. Tester dans Swagger

### Q: Comment utiliser la base de données?
**R:** Toujours via les repositories. Pas de DbContext direct dans les handlers!

### Q: Comment faire du caching?
**R:** Utiliser `IMemoryCache` ou `IDistributedCache` injecté dans le service.

### Q: Où mettre les validations?
**R:** 
- **Métier** → `PauperVault.Core/Domain/Rules`
- **API** → Attributes `[Required]`, `[StringLength]`
- **Domaine** → Dans les entities

## 💡 Bonnes pratiques

### ✅ À faire

```csharp
// 1. Utiliser les async/await
public async Task<Deck> GetDeckAsync(int id)
{
    return await _deckRepository.GetByIdAsync(id);
}

// 2. Valider les entrées
if (string.IsNullOrWhiteSpace(request.Name))
    throw new ValidationException("Name is required");

// 3. Utiliser les DTOs
public async Task<DeckResponse> Handle(GetDeckByIdQuery request)
{
    var deck = await _deckRepository.GetByIdAsync(request.Id);
    return _mapper.Map<DeckResponse>(deck);
}

// 4. Logger les erreurs
_logger.LogError(ex, "Failed to create deck: {Error}", ex.Message);

// 5. Gérer les exceptions
try { ... }
catch (DeckNotFoundException ex)
{
    return NotFound($"Deck {ex.DeckId} not found");
}
```

### ❌ À éviter

```csharp
// 1. ❌ Appels synchrones
var deck = _deckRepository.GetById(id);

// 2. ❌ Pas de validation
var name = request.Name; // Peut être null!

// 3. ❌ Entities dans les réponses
return Ok(deck); // Expose la structure DB

// 4. ❌ Swallow exceptions
try { ... } catch { }

// 5. ❌ DbContext dans les DTOs
public class DeckResponse
{
    public DbContext Context { get; set; } // ❌ Mauvais
}
```

## 🎓 Parcours d'apprentissage

### Jour 1 : Comprendre la structure
- [ ] Lire ce fichier (START_HERE.md)
- [ ] Explorer la structure des dossiers
- [ ] Lancer l'application

### Jour 2 : Explorer le code existant
- [ ] Lire une Entity (Card.cs, Deck.cs)
- [ ] Lire une Query & Handler
- [ ] Lire une Razor Page

### Jour 3 : Faire des modifications simples
- [ ] Corriger un bug mineur
- [ ] Ajouter une propriété
- [ ] Créer une migration

### Jour 4 : Créer une nouvelle fonctionnalité
- [ ] Créer une Query/Command
- [ ] Créer les tests
- [ ] Créer l'endpoint API

### Jour 5 : Réviser et améliorer
- [ ] Relire le code écrit
- [ ] Ajouter des tests
- [ ] Optimiser les performances

## 🚀 Prochaines étapes

1. **Installer** : Suivez [INSTALLATION.md](INSTALLATION.md)
2. **Comprendre** : Lisez [ARCHITECTURE.md](ARCHITECTURE.md)
3. **Contribuer** : Consultez [CONTRIBUTING.md](CONTRIBUTING.md)
4. **Coder** : Créez votre première feature!

---

**Dernière mise à jour :** 2026  
**Bienvenue dans l'équipe PauperVault! 🎉**
