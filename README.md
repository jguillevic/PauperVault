# 📚 PauperVault

> Une application web complète pour gérer votre collection de cartes **Magic: The Gathering** en format Pauper.

## 🎯 Vue d'ensemble

PauperVault est une plateforme moderne permettant aux joueurs de Magic: The Gathering de :
- 📊 Cataloguer leur collection de cartes Pauper
- 🎴 Créer et gérer des decks compétitifs
- 📈 Analyser les statistiques de leur collection
- 🔍 Rechercher et filtrer leurs cartes
- 💾 Exporter et partager leurs decks

## 🚀 Démarrage rapide

### Prérequis
- **.NET 9** ou supérieur
- **Visual Studio 2026** (Community, Professional, Enterprise)
- **SQL Server 2019+** ou **LocalDB**
- **Node.js 18+** (pour les assets front-end)

### Installation

```bash
# Cloner le repository
git clone https://github.com/jguillevic/PauperVault.git
cd PauperVault

# Restaurer les dépendances NuGet
dotnet restore

# Configurer la base de données
dotnet ef database update

# Démarrer l'application
dotnet run
```

L'application sera accessible sur `https://localhost:7001`

## 📂 Structure du projet

```
PauperVault/
├── PauperVault.Api/              # Backend API ASP.NET Core
│   ├── Features/                 # Features métier (CQRS)
│   ├── Infrastructure/           # Services, Extensions, Auth
│   └── Program.cs                # Configuration de l'app
├── PauperVault.Web/              # Frontend Razor Pages
│   ├── Pages/                    # Pages Razor
│   ├── wwwroot/                  # Assets (CSS, JS, images)
│   └── Components/               # Composants réutilisables
├── PauperVault.Core/             # Logique métier & domaine
│   ├── Domain/                   # Entities, Rules, Value Objects
│   └── Services/                 # Services de domaine
├── PauperVault.Contracts/        # DTOs et contrats partagés
└── Documentation/                # Guides et documentation
```

## 🏗️ Architecture

### Couches de l'application

```
┌─────────────────────────────────────────┐
│      Razor Pages (PauperVault.Web)      │ Présentation
├─────────────────────────────────────────┤
│    ASP.NET Core API (PauperVault.Api)   │ Application
├─────────────────────────────────────────┤
│    Domain (PauperVault.Core)            │ Domaine métier
├─────────────────────────────────────────┤
│    DTOs (PauperVault.Contracts)         │ Contrats
├─────────────────────────────────────────┤
│       Base de données (SQL Server)      │ Persistence
└─────────────────────────────────────────┘
```

### Pattern utilisés

- **CQRS** : Séparation Commandes/Requêtes
- **DDD** : Domain-Driven Design
- **Dependency Injection** : Conteneur IoC natif .NET
- **Repository Pattern** : Abstraction des données
- **Clean Code** : Principes SOLID appliqués

## 🔐 Authentification

- Authentification JWT
- Support OAuth2 (Azure AD compatible)
- Rôles et permissions basés sur les claims
- Connexion sécurisée HTTPS

## 🎨 Interface utilisateur

### Pages principales

- **Dashboard** : Vue d'ensemble de la collection
- **Mes Cartes** : Catalogage et gestion des cartes
- **Mes Decks** : Création et gestion des decks
  - **Créer un deck** (Edit.cshtml)
  - **Consulter un deck** (View.cshtml)
- **Recherche** : Filtrage avancé par propriétés

### Design

- ✅ Design System unifié
- ✅ Responsive (mobile, tablet, desktop)
- ✅ Support Dark Mode
- ✅ Accessibilité WCAG AA

Voir [DECK_UI_SUMMARY.md](DECK_UI_SUMMARY.md) pour les détails du design des pages Decks.

## 📋 Fonctionnalités principales

### Collection de cartes
- ✅ Ajouter/Supprimer des cartes
- ✅ Gérer les quantités
- ✅ Filtrer par type, couleur, coût
- ✅ Recherche textuelle
- ✅ Historique des modifications

### Gestion des Decks
- ✅ Créer des decks Pauper
- ✅ Valider la conformité des decks
- ✅ Calculer les statistiques
- ✅ Cloner/Dupliquer des decks
- ✅ Export en format standard

### Analyse
- ✅ Statistiques de collection
- ✅ Distribution des cartes par rareté
- ✅ Analyse des decks (courbe de mana, etc.)
- ✅ Comparaison de decks

## 🛠️ Technologies utilisées

### Backend
- **ASP.NET Core 9** : Framework web
- **Entity Framework Core** : ORM
- **SQL Server** : Base de données
- **AutoMapper** : Mappage d'objets
- **FluentValidation** : Validation
- **MediatR** : Implémentation CQRS

### Frontend
- **Razor Pages** : Rendu serveur
- **Bootstrap 5** : Framework CSS
- **CSS personnalisé** : Design système
- **JavaScript** : Interactions dynamiques
- **HTML5** : Sémantique

### Outils & Infrastructure
- **.NET 9** : Framework principal
- **Git** : Contrôle de version
- **Visual Studio 2026** : IDE
- **Azure** : Déploiement potentiel

## 📖 Documentation

- [START_HERE.md](START_HERE.md) - Guide de démarrage pour les développeurs
- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture détaillée
- [DECK_UI_GUIDE.md](DECK_UI_GUIDE.md) - Guide d'utilisation UI Decks
- [DECK_UI_MAINTENANCE.md](DECK_UI_MAINTENANCE.md) - Maintenance du design system
- [CHANGELOG.md](CHANGELOG.md) - Historique des changements
- [DELIVERY_NOTES.md](DELIVERY_NOTES.md) - Notes de livraison

## 🧪 Tests

```bash
# Exécuter tous les tests
dotnet test

# Exécuter les tests d'un projet
dotnet test PauperVault.Core.Tests

# Avec couverture de code
dotnet test /p:CollectCoverage=true
```

## 📊 Performance

- Temps de démarrage : < 2 secondes
- Temps de chargement page : < 500ms
- Support de 10K+ cartes
- Cache côté client

## 🔄 Cycle de développement

### Branching Strategy
```
main                    (Production)
  ├── staging          (Pre-production)
  └── develop          (Développement)
      ├── feature/*    (Nouvelles fonctionnalités)
      ├── bugfix/*     (Corrections de bugs)
      └── hotfix/*     (Corrections urgentes)
```

### Workflow Git

```bash
# Créer une branche de feature
git checkout -b feature/ma-fonctionnalite

# Faire les changements
# Committer régulièrement
git commit -m "feat: description de la modification"

# Créer une Pull Request
# Attendre la revue et les approbations
# Fusionner avec squash
```

## 🚀 Déploiement

### Environnements

| Environnement | URL | Branche |
|---------------|-----|--------|
| Local | http://localhost:7001 | - |
| Staging | https://staging.paupervault.com | staging |
| Production | https://paupervault.com | main |

### Checklist de déploiement

- [ ] Tous les tests passent
- [ ] Code review complète
- [ ] Documentation mise à jour
- [ ] CHANGELOG mis à jour
- [ ] Versioning (SemVer)
- [ ] Backup de la BD
- [ ] Plan de rollback prêt

## 🐛 Signaler un bug

Pour signaler un bug, créez une **GitHub Issue** avec :
- 📝 Description claire du problème
- 🔄 Étapes de reproduction
- 📷 Captures d'écran si applicable
- 💻 Environnement (OS, navigateur, version)
- 📋 Logs d'erreur si disponibles

## 💡 Proposer une fonctionnalité

1. Ouvrez une **Discussion** sur GitHub
2. Décrivez le besoin et les bénéfices
3. Discutez avec la communauté
4. Les propositions validées deviennent des Issues

## 🤝 Contribution

Les contributions sont bienvenues! Consultez [CONTRIBUTING.md](CONTRIBUTING.md) pour :
- Guidelines de contribution
- Style de code
- Processus de Pull Request
- Code of Conduct

## 📝 License

Ce projet est sous license [MIT](LICENSE). Voir le fichier LICENSE pour plus de détails.

## 👨‍💻 Auteur

**Julien Guillevic**
- GitHub: [@jguillevic](https://github.com/jguillevic)
- Email: contact@jguillevic.dev

## 🔗 Liens utiles

- [Magic: The Gathering](https://magic.wizards.com/)
- [Format Pauper](https://magic.wizards.com/en/formats/pauper)
- [Documentation ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## 💬 Support

Pour toute question ou support :
- 📧 Ouvrir une Issue sur GitHub
- 💬 Discuter sur les Discussions GitHub
- 📞 Contacter directement l'équipe

---

**Dernière mise à jour :** 2026 
**Version:** 1.0.0  
**Statut:** ✅ En production
