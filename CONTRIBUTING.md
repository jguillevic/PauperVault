# 🤝 Guide de Contribution

Merci de vouloir contribuer à **PauperVault**! Ce document vous guide à travers le processus de contribution.

## 📋 Table des matières

1. [Code of Conduct](#code-of-conduct)
2. [Comment contribuer](#comment-contribuer)
3. [Guide de style](#guide-de-style)
4. [Processus de Pull Request](#processus-de-pull-request)
5. [Workflow Git](#workflow-git)
6. [Standards de code](#standards-de-code)

## 🎯 Code of Conduct

### Notre engagement

Nous nous engageons à fournir un environnement accueillant et inclusif pour tous les contributeurs, indépendamment de :
- L'âge
- L'expérience
- L'origine
- L'identité
- La religion
- Les capacités
- L'orientation sexuelle

### Comportement attendu

**Contribuez de façon positive en :**
- ✅ Utilisant un langage inclusif
- ✅ Respectant les opinions différentes
- ✅ Acceptant les critiques constructives
- ✅ Vous concentrant sur ce qui est le mieux pour la communauté
- ✅ Montrant de l'empathie envers les autres

### Comportement inacceptable

**Ne pas:**
- ❌ Harceler ou intimider d'autres personnes
- ❌ Utiliser un langage offensant ou discriminatoire
- ❌ Publier des informations privées sans consentement
- ❌ Spam ou autocélébration excessive
- ❌ Trolling ou attaques personnelles

## 💡 Comment contribuer

### Les types de contributions acceptées

- **🐛 Corrections de bugs** : Fixes pour les problèmes existants
- **✨ Nouvelles fonctionnalités** : Améliorations et nouvelles capacités
- **📖 Documentation** : Guides, tutoriels, commentaires de code
- **🧪 Tests** : Cases de test, amélioration de la couverture
- **♿ Accessibilité** : Améliorations WCAG
- **⚡ Performance** : Optimisations et benchmarks
- **🎨 UI/UX** : Amélioration de l'interface utilisateur
- **🔒 Sécurité** : Vulnérabilités (responsable disclosure)

### Avant de commencer

1. **Vérifiez les issues existantes**
   - Peut-être que quelqu'un d'autre travaille déjà dessus
   - Consultez les PRs fermées pour le contexte

2. **Démarrez une discussion** (pour les grandes changements)
   - Ouvrez une Issue ou Discussion
   - Décrivez votre idée
   - Attendez les retours de la communauté

3. **Forker et cloner**
   ```bash
   git clone https://github.com/votre-username/PauperVault.git
   cd PauperVault
   git remote add upstream https://github.com/jguillevic/PauperVault.git
   ```

## 📝 Guide de style

### Noms de commits

Utilisez le format **Conventional Commits** :

```
<type>(<scope>): <description>

[body]

[footer]
```

#### Types valides

- `feat` : Nouvelle fonctionnalité
- `fix` : Correction de bug
- `docs` : Documentation
- `style` : Formatage (pas de changement de logique)
- `refactor` : Refactorisations sans feat/fix
- `perf` : Améliorations de performance
- `test` : Ajout ou modification de tests
- `chore` : Changements de build, dépendances, etc.
- `ci` : Changements de CI/CD

#### Exemples

```bash
git commit -m "feat(cards): ajouter recherche par type de mana"
git commit -m "fix(decks): corriger le calcul du nombre de cartes"
git commit -m "docs: ajouter guide d'installation"
git commit -m "style: formater le code selon eslint"
git commit -m "refactor(api): réorganiser les services"
git commit -m "test(core): améliorer couverture de DeckRules"
```

### Message de PR

```markdown
## 📝 Description
Brève description de ce qu'apporte cette PR

## 🎯 Type de changement
- [ ] 🐛 Correction de bug
- [ ] ✨ Nouvelle fonctionnalité
- [ ] 📖 Documentation
- [ ] 🧪 Tests
- [ ] ♿ Accessibilité
- [ ] ⚡ Performance

## 🔗 Issues liées
Ferme #123

## ✅ Checklist
- [ ] J'ai testé localement
- [ ] J'ai ajouté des tests
- [ ] J'ai mis à jour la documentation
- [ ] Pas d'erreurs de compilation
- [ ] Code review personnel fait
```

### Style de code C#

#### Conventions de nommage

```csharp
// Classes et Interfaces
public class CardRepository { }
public interface ICardService { }

// Méthodes et propriétés publiques
public string GetCardName() { }
public int CardCount { get; set; }

// Variables privées/locales
private string _cardName;
private int cardCount;

// Constants et statics
private const string DefaultDeckName = "Nouveau Deck";
private static readonly ILogger Logger;

// Async methods
public async Task<Card> GetCardAsync(int id) { }

// Events
public event EventHandler<CardEventArgs> CardAdded;
```

#### Formatage du code

```csharp
// ✅ BON
public class DeckService
{
    private readonly ICardRepository _cardRepository;
    
    public DeckService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
    
    public async Task<Deck> CreateDeckAsync(CreateDeckCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ValidationException("Le nom du deck est requis");
        
        var deck = new Deck { Name = command.Name };
        await _cardRepository.AddAsync(deck);
        return deck;
    }
}

// ❌ MAUVAIS
public class DeckService{
private ICardRepository cardRepository;
public DeckService(ICardRepository cardRepository){ this.cardRepository = cardRepository; }
public Deck CreateDeck(CreateDeckCommand cmd){
if(string.IsNullOrEmpty(cmd.Name)) throw new Exception("Name required");
var d = new Deck(); d.Name = cmd.Name;
cardRepository.Add(d); return d; }}
```

### Style CSS

```css
/* ✅ BON - BEM-like avec variables */
:root {
  --pv-primary: #1a1a1a;
  --pv-secondary: #f39c12;
  --pv-spacing-base: 1rem;
  --pv-radius: 8px;
}

.deck-card-table {
  background-color: var(--pv-bg);
  border-radius: var(--pv-radius);
  margin-bottom: var(--pv-spacing-base);
}

.deck-card-table__row {
  display: flex;
  gap: var(--pv-spacing-base);
}

.deck-card-table__row--highlight {
  background-color: var(--pv-highlight);
}

/* ❌ MAUVAIS */
.table { margin: 10px; }
.table-row { display: flex; }
.table-row-active { background: yellow; }
```

### Style HTML/Razor

```html
<!-- ✅ BON - Sémantique, accessibilité -->
<article class="deck-card-table">
  <header class="deck-section__header">
    <h2 class="deck-section__title">Cartes principales</h2>
    <span class="deck-section__count" aria-label="Nombre de cartes">60</span>
  </header>
  
  <table role="grid" aria-label="Liste des cartes du deck">
    <thead>
      <tr>
        <th scope="col">Quantité</th>
        <th scope="col">Carte</th>
        <th scope="col">Type</th>
        <th scope="col">Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td data-label="Quantité">1</td>
        <td data-label="Carte">Black Lotus</td>
        <td data-label="Type">Artifact</td>
        <td data-label="Actions">
          <button class="btn btn-danger" aria-label="Supprimer Black Lotus">✕</button>
        </td>
      </tr>
    </tbody>
  </table>
</article>

<!-- ❌ MAUVAIS -->
<div class="table">
  <div class="row">
    <div>1</div>
    <div>Black Lotus</div>
    <div><button onclick="delete()">Delete</button></div>
  </div>
</div>
```

## 🔄 Workflow Git

### Créer une branche pour votre contribution

```bash
# Mettre à jour votre fork
git fetch upstream
git checkout develop
git merge upstream/develop

# Créer une branche de feature
git checkout -b feature/description-courte

# Exemple:
git checkout -b feature/add-deck-statistics
git checkout -b bugfix/fix-card-search-crash
```

### Commits réguliers

```bash
# Commitez régulièrement avec messages clairs
git add .
git commit -m "feat(decks): ajouter calcul des statistiques"

# Peut aussi être des commits atomiques
git add PauperVault.Core/Domain/Decks/DeckStatistics.cs
git commit -m "feat(decks): créer classe DeckStatistics"

git add PauperVault.Api/Features/Decks/GetDeckStatisticsQuery.cs
git commit -m "feat(decks): ajouter query pour statistiques"
```

### Avant de pousser

```bash
# Vérifier que votre branche est à jour
git fetch upstream
git rebase upstream/develop

# Résoudre les conflits si nécessaire
# Tester localement
dotnet build
dotnet test

# Pousser vers votre fork
git push origin feature/description-courte
```

## 📤 Processus de Pull Request

### 1️⃣ Créer la PR

- Allez sur le repository principal
- Cliquez sur "New Pull Request"
- Sélectionnez votre branche
- Remplissez le template PR complètement

### 2️⃣ Attendre la revue

- Les mainteneurs vont revue le code
- Soyez patient et constructif
- Répondez aux commentaires

### 3️⃣ Apporter les modifications

```bash
# Apportez les changements demandés
git add .
git commit -m "refactor: appliquer les suggestions de revue"
git push origin feature/description-courte

# Les commits seront squashés lors de la merge
```

### 4️⃣ Merge

Une fois approuvée, votre PR sera fusionnée par un mainteneur.

## ✅ Standards de code

### Avant d'ouvrir une PR, assurez-vous que :

**Code quality**
- [ ] Le code compile sans erreurs : `dotnet build`
- [ ] Pas d'avertissements (warnings) : `dotnet build /p:TreatWarningsAsErrors=true`
- [ ] Respecte le style de code (voir ci-dessus)
- [ ] Pas de code mort ou dupliqué

**Tests**
- [ ] Les tests existants passent : `dotnet test`
- [ ] Nouveaux tests pour le nouveau code
- [ ] Couverture > 80% (si possible)

**Documentation**
- [ ] Code commenté pour la logique complexe
- [ ] Docstrings XML pour les méthodes publiques
- [ ] README mis à jour si besoin

**Sécurité**
- [ ] Pas de secrets (tokens, clés) en dur
- [ ] Validations des inputs
- [ ] Pas de SQL injection
- [ ] Pas de XSS

**Performance**
- [ ] Pas de N+1 queries
- [ ] Pas de boucles infinies
- [ ] Pas de memory leaks évidents

### Exemple de PR complète

```csharp
/// <summary>
/// Calcule les statistiques d'un deck.
/// </summary>
/// <param name="deckId">Identifiant du deck</param>
/// <returns>Statistiques du deck</returns>
/// <exception cref="DeckNotFoundException">Si le deck n'existe pas</exception>
public async Task<DeckStatistics> GetStatisticsAsync(int deckId)
{
    var deck = await _deckRepository.GetByIdAsync(deckId)
        ?? throw new DeckNotFoundException(deckId);
    
    var statistics = new DeckStatistics
    {
        TotalCards = deck.Cards.Sum(c => c.Quantity),
        AverageMana = CalculateAverageMana(deck),
        ColorDistribution = CalculateColorDistribution(deck)
    };
    
    return statistics;
}

[Test]
public async Task GetStatisticsAsync_WithValidDeck_ReturnsCorrectStatistics()
{
    // Arrange
    var deckId = 1;
    var deck = new Deck { /* ... */ };
    _deckRepository.Setup(x => x.GetByIdAsync(deckId)).ReturnsAsync(deck);
    
    // Act
    var result = await _deckService.GetStatisticsAsync(deckId);
    
    // Assert
    Assert.That(result.TotalCards, Is.EqualTo(60));
    Assert.That(result.AverageMana, Is.GreaterThan(0));
}
```

## 📚 Ressources

### Documentation du projet
- [START_HERE.md](START_HERE.md) - Démarrage pour développeurs
- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture globale
- [CHANGELOG.md](CHANGELOG.md) - Historique des versions

### Documentation externe
- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Microsoft ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [BEM CSS Methodology](http://getbem.com/)
- [Semantic Versioning](https://semver.org/)
- [Conventional Commits](https://www.conventionalcommits.org/)

## ❓ Questions?

Si vous avez des questions :
- 💬 Ouvrez une Discussion sur GitHub
- 🐛 Consultez les Issues existantes
- 📧 Contactez les mainteneurs
- 📖 Lisez la documentation

## 🙏 Merci!

Votre contribution, quelle qu'elle soit, aide à améliorer PauperVault pour toute la communauté.

Chaque commit compte, du code aux docs en passant par les reports de bugs.

**Bienvenue dans l'équipe PauperVault! 🎉**

---

**Dernière mise à jour :** 2026
**Version:** 1.0  
**Mainteneur:** Jérôme Guillevic
