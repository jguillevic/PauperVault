# 🏗️ Architecture Technique Détaillée

Documentation complète de l'architecture technique de **PauperVault**.

## 📋 Table des matières

1. [Vue d'ensemble](#vue-densemble)
2. [Architecture en couches](#architecture-en-couches)
3. [Patterns utilisés](#patterns-utilisés)
4. [Structure des projets](#structure-des-projets)
5. [Flux de données](#flux-de-données)
6. [Domain-Driven Design](#domain-driven-design)
7. [Sécurité](#sécurité)
8. [Performance](#performance)
9. [Scalabilité](#scalabilité)

## 🎯 Vue d'ensemble

### Principes directeurs

```
┌─────────────────────────────────────────────────────────────┐
│                  PRINCIPES D'ARCHITECTURE                   │
├─────────────────────────────────────────────────────────────┤
│ 1. Domain-Driven Design (DDD)                               │
│    └─ Logique métier centralisée dans le Domain             │
│                                                               │
│ 2. CQRS (Command Query Responsibility Segregation)          │
│    └─ Séparation des opérations de lecture/écriture        │
│                                                               │
│ 3. Dependency Injection                                     │
│    └─ Couplage loose, testabilité élevée                   │
│                                                               │
│ 4. Clean Code / SOLID                                       │
│    └─ S - Single Responsibility                             │
│    └─ O - Open/Closed                                       │
│    └─ L - Liskov Substitution                               │
│    └─ I - Interface Segregation                             │
│    └─ D - Dependency Inversion                              │
│                                                               │
│ 5. Separation of Concerns                                   │
│    └─ Chaque couche a une responsabilité unique             │
└─────────────────────────────────────────────────────────────┘
```

## 🏢 Architecture en couches

### Vue complète

```
┌────────────────────────────────────────────────┐
│      COUCHE PRÉSENTATION (Razor Pages)         │  PauperVault.Web
│  ┌─────────────────────────────────────────┐   │
│  │ Pages/Edit.cshtml                       │   │ Gestion des Decks
│  │ Pages/View.cshtml                       │   │ Création & Edition
│  │ Pages/Dashboard.cshtml                  │   │ Affichage
│  │ + Components réutilisables              │   │
│  └─────────────────────────────────────────┘   │
└────────────────────────────────────────────────┘
                      ↓
┌────────────────────────────────────────────────┐
│   COUCHE APPLICATION (API Controllers)         │  PauperVault.Api
│  ┌─────────────────────────────────────────┐   │
│  │ Features/Decks/Commands/                │   │ Orchestration
│  │ Features/Decks/Queries/                 │   │ Validation
│  │ Features/Cards/Commands/                │   │ Mapping
│  │ Features/Cards/Queries/                 │   │
│  │ (CQRS pattern avec MediatR)             │   │
│  └─────────────────────────────────────────┘   │
└────────────────────────────────────────────────┘
                      ↓
┌────────────────────────────────────────────────┐
│    COUCHE DOMAINE (Logique métier)            │  PauperVault.Core
│  ┌─────────────────────────────────────────┐   │
│  │ Domain/Cards/                           │   │ Entities
│  │  └─ Card, CardRules                     │   │ Value Objects
│  │ Domain/Decks/                           │   │ Business Logic
│  │  └─ Deck, DeckRules, DeckStatistics     │   │ Rules
│  │ Services/                               │   │ Services
│  │  └─ CardService, DeckService            │   │
│  └─────────────────────────────────────────┘   │
└────────────────────────────────────────────────┘
                      ↓
┌────────────────────────────────────────────────┐
│   COUCHE INFRASTRUCTURE (Data & External)     │  PauperVault.Api
│  ┌─────────────────────────────────────────┐   │
│  │ Infrastructure/                         │   │ Repositories
│  │  └─ Persistence/                        │   │ EF Core DbContext
│  │  └─ Authentication/JWT                  │   │ Services tiers
│  │  └─ Extensions/                         │   │ Configuration
│  │  └─ Middleware/                         │   │
│  └─────────────────────────────────────────┘   │
└────────────────────────────────────────────────┘
                      ↓
┌────────────────────────────────────────────────┐
│       COUCHE CONTRATS (Partagée)               │  PauperVault.Contracts
│  ┌─────────────────────────────────────────┐   │
│  │ Requests/ (DTO pour les commandes)      │   │ DTOs
│  │ Responses/ (DTO pour les réponses)      │   │ Contrats
│  │ Common/ (Objets partagés)               │   │
│  └─────────────────────────────────────────┘   │
└────────────────────────────────────────────────┘
                      ↓
┌────────────────────────────────────────────────┐
│     SOURCES EXTERNES & PERSISTENCE             │
│  ┌─────────────────────────────────────────┐   │
│  │ SQL Server / Entity Framework Core      │   │
│  │ Authentication (JWT, OAuth2)            │   │
│  │ File System (Uploads, Exports)          │   │
│  │ External APIs (Scryfall, etc.)          │   │
│  └─────────────────────────────────────────┘   │
└────────────────────────────────────────────────┘
```

## 🔄 Patterns utilisés

### 1. CQRS (Command Query Responsibility Segregation)

```
COMMANDES (Écritures)
│
├─ CreateDeckCommand
│  └─ CreateDeckCommandHandler
│     └─ Valide → Crée l'entité → Persiste → Retourne
│
├─ UpdateDeckCommand
│  └─ UpdateDeckCommandHandler
│
└─ DeleteDeckCommand
   └─ DeleteDeckCommandHandler

REQUÊTES (Lectures)
│
├─ GetDeckByIdQuery
│  └─ GetDeckByIdQueryHandler
│     └─ Fetch depuis la BD → Mappe → Retourne
│
├─ GetAllDecksQuery
│  └─ GetAllDecksQueryHandler
│
└─ GetDeckStatisticsQuery
   └─ GetDeckStatisticsQueryHandler
```

#### Avantages
- ✅ Séparation claire lecture/écriture
- ✅ Scalabilité (différentes BDs pour read/write)
- ✅ Testabilité
- ✅ Optimisation indépendante

### 2. Domain-Driven Design (DDD)

```
UBIQUITOUS LANGUAGE (Langage partagé)
│
├─ "Deck" = Collection de cartes réglementées
├─ "Card" = Composant du deck
├─ "MainDeck" = Cartes jouables
├─ "Sideboard" = Cartes interchangeables
└─ "Pauper" = Format avec cartes rares exclus

BOUNDED CONTEXTS (Contextes délimités)
│
├─ CARDS CONTEXT
│  ├─ Entity: Card
│  ├─ ValueObject: CardCost, CardType
│  └─ Rules: CardRules (validations)
│
└─ DECKS CONTEXT
   ├─ Aggregate Root: Deck
   ├─ Entities: DeckCard, MainSection, Sideboard
   ├─ ValueObjects: DeckStatistics, DeckFormat
   └─ Rules: DeckRules (conformité Pauper)
```

#### Structure DDD du projet

```
PauperVault.Core/Domain/
│
├─ Cards/
│  ├─ Card.cs (Entity)
│  ├─ CardCost.cs (Value Object)
│  ├─ CardType.cs (Value Object)
│  ├─ CardRarity.cs (Enum)
│  ├─ CardColor.cs (Enum)
│  ├─ CardRules.cs (Business Logic)
│  └─ ICardRepository.cs (Repository Interface)
│
├─ Decks/
│  ├─ Deck.cs (Aggregate Root)
│  ├─ DeckCard.cs (Entity)
│  ├─ DeckFormat.cs (Value Object)
│  ├─ DeckRules.cs (Business Logic)
│  ├─ DeckStatistics.cs (Service)
│  └─ IDeckRepository.cs (Repository Interface)
│
└─ Exceptions/
   ├─ InvalidDeckException
   ├─ CardNotFoundException
   └─ DeckRulesViolationException
```

### 3. Repository Pattern

```csharp
// Interface (Infrastructure/Persistence)
public interface IDeckRepository
{
    Task<Deck> GetByIdAsync(int id);
    Task<IEnumerable<Deck>> GetAllAsync();
    Task AddAsync(Deck deck);
    Task UpdateAsync(Deck deck);
    Task DeleteAsync(int id);
}

// Implémentation (EF Core)
public class DeckRepository : IDeckRepository
{
    private readonly PauperVaultDbContext _context;
    
    public async Task<Deck> GetByIdAsync(int id)
    {
        return await _context.Decks
            .Include(d => d.Cards)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}
```

### 4. Specification Pattern (Requêtes complexes)

```csharp
// Plutôt que N repository methods:
// GetByFormatAndColorAsync
// GetByRarityRangeAsync
// GetExpensiveCardsAsync

// On utilise des spécifications:
var spec = new CardSpecification()
    .WithFormat(CardFormat.Pauper)
    .WithColor(CardColor.Blue)
    .WithCostRange(1, 3);

var cards = await _cardRepository.GetAsync(spec);
```

### 5. Builder Pattern (Création d'objets complexes)

```csharp
var deck = new DeckBuilder()
    .WithName("My Pauper Deck")
    .WithFormat(DeckFormat.Pauper)
    .AddCard(card1, quantity: 2)
    .AddCard(card2, quantity: 1)
    .AddSideboardCard(card3, quantity: 3)
    .Build();
```

## 📂 Structure des projets

### PauperVault.Core

```
PauperVault.Core/
│
├─ Domain/                    # Logique métier
│  ├─ Cards/
│  │  ├─ Card.cs              # Entity principal
│  │  ├─ CardRules.cs         # Validations métier
│  │  ├─ ICardRepository.cs    # Interface repository
│  │  └─ *.cs                 # Value Objects, Enums
│  │
│  ├─ Decks/
│  │  ├─ Deck.cs              # Aggregate Root
│  │  ├─ DeckRules.cs         # Conformité Pauper
│  │  ├─ DeckStatistics.cs    # Calculs
│  │  └─ IDeckRepository.cs
│  │
│  └─ Exceptions/
│     └─ *.cs                 # Exceptions métier
│
└─ Services/                  # Services de domaine
   ├─ CardService.cs
   └─ DeckService.cs
```

### PauperVault.Api

```
PauperVault.Api/
│
├─ Features/                  # CQRS
│  ├─ Cards/
│  │  ├─ Queries/
│  │  │  ├─ GetCardByIdQuery.cs
│  │  │  ├─ GetCardByIdQueryHandler.cs
│  │  │  └─ GetAllCardsQuery.cs
│  │  │
│  │  └─ Commands/
│  │     ├─ CreateCardCommand.cs
│  │     ├─ CreateCardCommandHandler.cs
│  │     └─ UpdateCardCommand.cs
│  │
│  └─ Decks/
│     ├─ Queries/
│     │  ├─ GetDeckByIdQuery.cs
│     │  ├─ GetDeckByIdQueryHandler.cs
│     │  └─ GetDeckStatisticsQuery.cs
│     │
│     └─ Commands/
│        ├─ CreateDeckCommand.cs
│        ├─ CreateDeckCommandHandler.cs
│        ├─ AddCardToDeckCommand.cs
│        └─ AddCardToDeckCommandHandler.cs
│
├─ Controllers/               # API Endpoints
│  ├─ CardsController.cs
│  ├─ DecksController.cs
│  └─ HealthController.cs
│
├─ Infrastructure/
│  ├─ Persistence/
│  │  ├─ PauperVaultDbContext.cs
│  │  ├─ CardRepository.cs
│  │  ├─ DeckRepository.cs
│  │  └─ Migrations/
│  │
│  ├─ Authentication/
│  │  ├─ JwtTokenGenerator.cs
│  │  └─ UserClaimsExtensions.cs
│  │
│  ├─ Extensions/
│  │  ├─ ServiceCollectionExtensions.cs
│  │  ├─ DatabaseServiceExtensions.cs
│  │  ├─ AuthenticationServiceExtensions.cs
│  │  └─ ApplicationPipelineExtensions.cs
│  │
│  ├─ Middleware/
│  │  ├─ GlobalExceptionHandlingMiddleware.cs
│  │  ├─ RequestLoggingMiddleware.cs
│  │  └─ AuthenticationMiddleware.cs
│  │
│  └─ Configuration/
│     └─ *.cs
│
├─ Program.cs                # Entry point
└─ appsettings.json         # Configuration
```

### PauperVault.Web

```
PauperVault.Web/
│
├─ Pages/                    # Razor Pages
│  ├─ Decks/
│  │  ├─ Index.cshtml       # Liste des decks
│  │  ├─ Index.cshtml.cs    # Page handler
│  │  ├─ Create.cshtml      # Créer un deck
│  │  ├─ Create.cshtml.cs
│  │  ├─ Edit.cshtml        # Éditer un deck
│  │  ├─ Edit.cshtml.cs
│  │  ├─ View.cshtml        # Consulter un deck
│  │  └─ View.cshtml.cs
│  │
│  ├─ Cards/
│  │  ├─ Index.cshtml
│  │  ├─ Index.cshtml.cs
│  │  └─ ...
│  │
│  ├─ Dashboard/
│  │  ├─ Index.cshtml
│  │  └─ Index.cshtml.cs
│  │
│  └─ Shared/
│     ├─ _Layout.cshtml      # Layout principal
│     ├─ _Navigation.cshtml  # Navigation
│     └─ _LoginPartial.cshtml
│
├─ Components/              # Composants réutilisables
│  ├─ CardPreview.razor
│  ├─ DeckStats.razor
│  └─ *.razor
│
├─ wwwroot/                 # Assets statiques
│  ├─ css/
│  │  ├─ site.css           # Styles principaux
│  │  └─ DeckShared.css     # Design System Decks
│  │
│  ├─ js/
│  │  ├─ site.js
│  │  └─ deck-editor.js
│  │
│  ├─ lib/                  # Libraries externes
│  │  ├─ bootstrap/
│  │  ├─ jquery/
│  │  └─ ...
│  │
│  └─ images/
│     ├─ cards/
│     └─ icons/
│
└─ Program.cs              # Configuration Web
```

### PauperVault.Contracts

```
PauperVault.Contracts/
│
├─ Requests/               # DTOs pour les commandes
│  ├─ CreateDeckRequest.cs
│  ├─ UpdateDeckRequest.cs
│  ├─ AddCardToDeckRequest.cs
│  └─ ...
│
├─ Responses/              # DTOs pour les réponses
│  ├─ DeckResponse.cs
│  ├─ CardResponse.cs
│  ├─ DeckStatisticsResponse.cs
│  └─ ...
│
├─ Common/                 # Objets partagés
│  ├─ ApiResponse.cs
│  ├─ PagedResult.cs
│  ├─ ErrorResponse.cs
│  └─ ...
│
└─ Constants/              # Constantes métier
   ├─ DeckConstraints.cs
   ├─ CardFormats.cs
   └─ ...
```

## 🔀 Flux de données

### Exemple : Créer un Deck

```
┌─────────────────┐
│  Client Browser │
│  (Razor Page)   │
└────────┬────────┘
         │ POST /api/decks
         ↓
┌─────────────────────────────┐
│ DecksController.CreateAsync │
│  - Reçoit CreateDeckRequest │
└────────┬────────────────────┘
         │ MediatR.Send(CreateDeckCommand)
         ↓
┌──────────────────────────────────────┐
│ CreateDeckCommandHandler             │
│  - Valide les données                │
│  - Vérifie les règles métier         │
└────────┬─────────────────────────────┘
         │ DeckRules.ValidateNewDeck()
         ↓
┌──────────────────────────────────────┐
│ DeckRules (Domain)                   │
│  - Applique les règles Pauper        │
│  - Vérifie les contraintes           │
└────────┬─────────────────────────────┘
         │ _deckRepository.AddAsync(deck)
         ↓
┌──────────────────────────────────────┐
│ DeckRepository (Infrastructure)      │
│  - Crée l'entité EF Core             │
│  - Ajoute au DbContext               │
└────────┬─────────────────────────────┘
         │ _context.SaveChangesAsync()
         ↓
┌──────────────────────────────────────┐
│ SQL Server                           │
│  - INSERT INTO Decks ...             │
│  - Transaction ACID                  │
└────────┬─────────────────────────────┘
         │ Success (Id = 123)
         ↓
┌──────────────────────────────────────┐
│ CreateDeckCommandHandler             │
│  - Mappe vers DeckResponse           │
│  - Retourne 201 Created              │
└────────┬─────────────────────────────┘
         │ { id: 123, name: "My Deck", ... }
         ↓
┌─────────────────┐
│  Client Browser │
│  Reçoit réponse │
└─────────────────┘
```

### Exemple : Consulter un Deck

```
┌─────────────────┐
│  Client Browser │
│  GET /decks/123 │
└────────┬────────┘
         │
         ↓
┌─────────────────────────────────┐
│ DecksPageModel.OnGet(id)        │
│  (Razor Page Handler)           │
└────────┬────────────────────────┘
         │ _deckService.GetByIdAsync(123)
         ↓
┌──────────────────────────────────┐
│ GetDeckByIdQuery                 │
│  - Spécifie le deck à récupérer  │
└────────┬───────────────────────┘
         │ MediatR.Send(query)
         ↓
┌──────────────────────────────────┐
│ GetDeckByIdQueryHandler          │
│  - Récupère depuis la BD         │
└────────┬───────────────────────┘
         │ _deckRepository.GetByIdAsync(123)
         ↓
┌──────────────────────────────────┐
│ DeckRepository                   │
│  - EF Core Query                 │
│  - Include(d => d.Cards)         │
└────────┬───────────────────────┘
         │ SELECT * FROM Decks WHERE Id=123
         ↓
┌──────────────────────────────────┐
│ SQL Server                       │
│  - Retourne les données          │
└────────┬───────────────────────┘
         │ Deck object + Cards
         ↓
┌──────────────────────────────────┐
│ GetDeckByIdQueryHandler          │
│  - Mappe à DeckViewModel         │
│  - Retourne au PageModel         │
└────────┬───────────────────────┘
         │ return new DeckViewModel { ... }
         ↓
┌─────────────────────────────┐
│ Razor View (View.cshtml)    │
│  - Rend le HTML             │
│  - Affiche le deck          │
└────────┬────────────────────┘
         │ HTML rendu
         ↓
┌─────────────────┐
│  Client Browser │
│  Affiche page   │
└─────────────────┘
```

## 🔐 Authentification & Autorisation

### JWT Token Flow

```
┌─────────────────────┐
│  Utilisateur        │
│  Login + Password   │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────────────────────────┐
│  AuthController.LoginAsync              │
│  - Valide credentials                   │
│  - Recherche l'utilisateur              │
└──────────┬──────────────────────────────┘
           │
           ↓
┌─────────────────────────────────────────┐
│  JwtTokenGenerator                      │
│  - Crée les claims                      │
│  - Signe le token                       │
│  - Retourne JWT                         │
└──────────┬──────────────────────────────┘
           │ eyJhbGciOiJIUzI1NiIsInR...
           ↓
┌─────────────────────────────────────────┐
│  Client                                 │
│  Stocke le token (localStorage)         │
│  Envoie dans les headers                │
│  Authorization: Bearer <token>          │
└──────────┬──────────────────────────────┘
           │
           ↓
┌─────────────────────────────────────────┐
│  AuthenticationMiddleware               │
│  - Valide la signature                  │
│  - Extrait les claims                   │
│  - Crée ClaimsPrincipal                 │
└──────────┬──────────────────────────────┘
           │
           ↓
┌─────────────────────────────────────────┐
│  [Authorize] Controller                 │
│  - Vérifie les permissions              │
│  - Exécute la requête                   │
└─────────────────────────────────────────┘
```

### Rôles & Permissions

```csharp
// Claims utilisateur
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, "User"),           // Rôle
    new Claim("DeckId", "123"),                   // Permission spécifique
};

// Utilisation dans les controllers
[Authorize(Roles = "Admin")]
public IActionResult DeleteDeck(int id) { ... }

[Authorize]
[Policy("CanEditDeck")]
public IActionResult EditDeck(int id) { ... }
```

## ⚡ Performance

### Optimisations implémentées

#### 1. Database Queries
```csharp
// ❌ N+1 Queries
var decks = await _context.Decks.ToListAsync();
foreach (var deck in decks)
{
    var cards = await _context.DeckCards
        .Where(dc => dc.DeckId == deck.Id)
        .ToListAsync(); // 1 requête par deck
}

// ✅ Single Query avec Include
var decks = await _context.Decks
    .Include(d => d.Cards)     // Eager loading
    .ThenInclude(c => c.Card)
    .ToListAsync();            // 1 requête totale
```

#### 2. Caching
```csharp
// Redis Cache (optionnel)
var cachedDeck = await _cache.GetStringAsync("deck:123");
if (cachedDeck == null)
{
    var deck = await _deckRepository.GetByIdAsync(123);
    await _cache.SetStringAsync("deck:123", JsonConvert.SerializeObject(deck));
}
```

#### 3. Pagination
```csharp
// Au lieu de charger toutes les cartes
var cards = await _cardRepository
    .GetAllAsync()
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

#### 4. Projections
```csharp
// ❌ Charger l'entité complète
var decks = await _context.Decks.ToListAsync();

// ✅ Projection vers DTO
var deckDtos = await _context.Decks
    .Select(d => new DeckDto
    {
        Id = d.Id,
        Name = d.Name,
        CardCount = d.Cards.Count()
    })
    .ToListAsync();
```

### Métriques cibles

| Métrique | Cible | Méthode de test |
|----------|-------|-----------------|
| Temps de démarrage | < 2s | Benchmark |
| Chargement page | < 500ms | Lighthouse |
| API response | < 200ms | Load test |
| Memory usage | < 500MB | Profiler |

## 🚀 Scalabilité

### Croissance horizontale

```
┌─────────────────────────────────────────┐
│  Load Balancer (Azure App Gateway)      │
└────────┬─────────────────────┬──────────┘
         │                     │
    ┌────▼────┐           ┌────▼────┐
    │ Instance │           │ Instance │
    │   Web 1  │           │   Web 2  │
    └────┬────┘           └────┬────┘
         │                     │
         └──────────┬──────────┘
                    │
            ┌───────▼────────┐
            │ SQL Server     │
            │ (Azure SQL DB) │
            └────────────────┘
```

### Séparation des préoccupations

- **Read Replicas** : Base de données de lecture pour les requêtes
- **Caching** : Redis/Azure Cache pour les données fréquentes
- **Queue** : Azure Service Bus pour les tâches asynchrones
- **CDN** : Azure CDN pour les assets statiques

## 🏁 Conclusion

L'architecture de PauperVault:
- ✅ Est flexible et évolutive
- ✅ Respecte les principes SOLID
- ✅ Est maintenable et testable
- ✅ Supporte la croissance
- ✅ Permet l'évolution technologique

---

**Dernière mise à jour :** 2026
**Version :** 1.0  
**Responsable :** Jérôme Guillevic
