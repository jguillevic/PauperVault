# PauperVault.Api.Tests

Suite de tests unitaires pour le projet `PauperVault.Api`.

## Structure des Tests

Les tests sont organisés par module/fonctionnalité :

### Infrastructure Tests

#### Auth
- **UserClaimsExtensionsTests** : Tests pour l'extraction des claims JWT
  - Récupération de l'ID utilisateur (avec et sans exception)
  - Récupération de l'email de l'utilisateur
  
- **JwtTokenGeneratorTests** : Tests pour la génération des tokens JWT
  - Génération valide avec différentes configurations
  - Inclusion correcte des claims
  - Gestion des durées d'expiration

#### Configuration
- **ConfigurationKeysTests** : Tests des clés de configuration statiques
  - Vérification de toutes les constantes de configuration
  - Validité des clés de chaîne de connexion
  - Configuration JWT, Scryfall, et logging

#### Scryfall
- **ScryfallModelsTests** : Tests des modèles DTO Scryfall
  - Sérialisation/désérialisation des réponses Scryfall
  - Gestion des images et faces de cartes
  - Métadonnées des cartes

### Features Tests

#### Auth
- **AuthCommandResultTests** : Tests des enregistrements de résultats d'authentification
  - Success avec token et/ou message
  - ValidationError, Unauthorized, ConfigurationError, Conflict
  - AuthErrorItem

- **AuthMappingsTests** : Tests du mapping des résultats
  - Conversion des erreurs IdentityResult
  - Mapping vers les résultats HTTP

- **GoogleLoginCommandResultTests** : Tests des résultats de connexion Google
  - Success avec token
  - ValidationError avec payload

#### Cards
- **CardQueriesTests** : Tests des requêtes de cartes
  - Autocomplete vide
  - DTO d'autocomplete

## Exécution des Tests

Tous les tests peuvent être exécutés via Visual Studio Test Explorer ou la ligne de commande :

```bash
dotnet test PauperVault.Api.Tests
```

Pour exécuter un test spécifique :

```bash
dotnet test PauperVault.Api.Tests --filter "TestClassName.TestMethodName"
```

## Configuration

Le projet utilise :
- **Framework** : MSTest 4.0.2
- **Mocking** : Moq 4.20.72
- **Framework cible** : .NET 9.0

## Couverture

Les tests couvrent actuellement :
- 57 test cases
- Tous les modèles et utilitaires d'infrastructure
- Les mappages et transformations de données
- Les résultats de commandes d'authentification
- Les requêtes de cartes

## À ajouter

Futures extensions potentielles :
- Tests des handlers avec dépendances mockées
- Tests des endpoints avec HttpClient factory
- Tests d'intégration de base de données
- Tests pour CardCommands et DeckCommands avec mocks
