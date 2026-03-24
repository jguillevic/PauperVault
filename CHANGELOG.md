# 🔄 Historique des changements (CHANGELOG)

Toutes les modifications notables du projet **PauperVault** sont documentées dans ce fichier.

Le format suit [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/) et le versioning [Semantic Versioning](https://semver.org/lang/fr/).

## [1.0.0] - 2024-01-XX

### ✨ Ajoutés (Features)

#### Gestion des Decks
- ✅ Création de nouveaux decks Pauper
- ✅ Édition des decks existants
- ✅ Suppression des decks
- ✅ Vue détaillée des decks avec statistiques
- ✅ Support du MainDeck et Sideboard
- ✅ Validation automatique des règles Pauper

#### Gestion des Cartes
- ✅ Catalogue complet de cartes
- ✅ Recherche avancée par propriétés
- ✅ Filtrage par type, couleur, coût
- ✅ Gestion des quantités
- ✅ Import de cartes externe

#### Statistiques & Analyse
- ✅ Calcul des statistiques deck (mana, types, etc.)
- ✅ Distribution des couleurs
- ✅ Courbe de mana
- ✅ Vue d'ensemble de la collection

#### Authentification & Sécurité
- ✅ Authentification JWT
- ✅ Gestion des utilisateurs
- ✅ Rôles et permissions
- ✅ Sessions sécurisées

#### Interface Utilisateur
- ✅ Design System unifié pour Decks
- ✅ Interface responsive (mobile, tablet, desktop)
- ✅ Support Dark Mode complet
- ✅ Accessibilité WCAG AA
- ✅ Composants réutilisables

#### Documentation
- ✅ Guide de démarrage pour développeurs
- ✅ Documentation architecture détaillée
- ✅ Guide d'installation complet
- ✅ Guide de contribution
- ✅ Documentation du design system
- ✅ Maintenance guide

#### Tests
- ✅ Tests unitaires des règles métier
- ✅ Tests d'intégration
- ✅ Tests de couverture

### 🔧 Changé (Changes)

- Restructuration complète en architecture DDD
- Migration vers ASP.NET Core 9
- Implémentation du pattern CQRS avec MediatR
- Refonte de la base de données (EF Core migrations)
- Modernisation du CSS (variables CSS, BEM naming)
- Unification du design Edit.cshtml et View.cshtml

### 🔒 Sécurisé (Security)

- ✅ Validation CSRF (CSRF tokens)
- ✅ Protection XSS (HTML encoding)
- ✅ Validation des entrées
- ✅ Hachage des mots de passe (bcrypt)
- ✅ CORS configuration
- ✅ Secrets management

### 🗑️ Dépréciés (Deprecated)

- Ancienne API REST (sera supprimée en 2.0)
- Authentification par formulaire (remplacée par JWT)

### 🐛 Corrigé (Fixed)

- Correction du crash lors de la recherche de cartes
- Correction de la validation du Pauper format
- Correction du calcul de mana courbe
- Correction des bugs d'affichage responsive
- Correction de la pagination des cartes

### ⚠️ À noter (Breaking Changes)

⚠️ **API Response Format**
L'ancienne structure API a changé:

```json
// ❌ Ancien format
{
    "success": true,
    "data": { "id": 1, "name": "Deck" }
}

// ✅ Nouveau format (2024-01-XX)
{
    "value": { "id": 1, "name": "Deck" },
    "errors": []
}
```

**Migration**: Mettre à jour vos clients API.

---

## [0.9.0] - 2023-12-XX (Beta)

### ✨ Ajoutés

- Support initial des decks
- Recherche basique de cartes
- Interface utilisateur Razor Pages
- Authentification basique

### 🐛 Corrigé

- Performance des requêtes database
- Validations du format Pauper
- Responsive design sur mobile

### ⚠️ À noter

- Version bêta, certaines fonctionnalités incomplètes
- Pas prêt pour la production

---

## Conventions de versioning

### Format: MAJEURE.MINEURE.PATCH

- **MAJEURE** : Breaking changes (0 → 1 = gros changements)
- **MINEURE** : Nouvelles fonctionnalités (compatibles)
- **PATCH** : Corrections de bugs

### Exemples

- `1.0.0` : Première version stable
- `1.1.0` : Nouvelle feature (compatible)
- `1.1.1` : Correction d'un bug
- `2.0.0` : Breaking changes

## Format des changements

### Types de changements

```markdown
### ✨ Ajoutés (Added)
- Nouvelles fonctionnalités

### 🔧 Changé (Changed)
- Comportements modifiés

### 🗑️ Dépréciés (Deprecated)
- À éviter, sera supprimé

### 🚀 Enlevés (Removed)
- Supprimé de la version

### 🐛 Corrigé (Fixed)
- Corrections de bugs

### 🔒 Sécurisé (Security)
- Failles de sécurité
```

## Guide pour contribuer au CHANGELOG

Lors de chaque Pull Request:

1. **Ajouter une ligne au CHANGELOG**
   ```markdown
   ### [Unreleased]
   ### Ajoutés
   - Votre nouvelle feature
   ```

2. **Respecter le format**
   - Section appropriée
   - Phrases brèves et claires
   - Liens vers les PRs si disponibles

3. **Lors du release**
   - Créer une section `[X.Y.Z] - YYYY-MM-DD`
   - Ajouter le lien de comparaison GitHub

## Liens de comparaison

Ajouter des liens pour chaque version:

```markdown
[1.0.0]: https://github.com/jguillevic/PauperVault/releases/tag/v1.0.0
[0.9.0]: https://github.com/jguillevic/PauperVault/compare/v0.8.0...v0.9.0
```

## Notes pour les utilisateurs

### Mise à jour recommandée
Pour les versions avec `🐛 Corrigé` = mise à jour recommandée

### Mise à jour obligatoire
Pour les versions avec `🔒 Sécurisé` = mise à jour IMPORTANTE

### Compatibilité
- ✅ `1.0.x` → `1.0.y` : Remontée recommandée
- ✅ `1.x.0` → `1.y.0` : Remontée recommandée
- ⚠️ `1.x.0` → `2.0.0` : Changements majeurs, lire les notes

## Roadmap

### Prochaines features (v1.1.0)

- 🎯 Export deck en format standard
- 🎯 Partage de decks
- 🎯 Historique des révisions
- 🎯 Statistiques avancées

### Prochaines features (v2.0.0)

- 🎯 Éditeur visuel de decks
- 🎯 Intégration API Scryfall
- 🎯 Partie multijoueur (local)
- 🎯 Grinder de mana
- 🎯 Support mobile app

### Non planifiés (pour le moment)

- ❌ Vente de cartes
- ❌ Matchmaking en ligne
- ❌ Smart contracts / blockchain

---

## Comment lire le CHANGELOG?

```
[VERSION] - DATE
└─ Section
   └─ • Description
```

**Exemple:**
```
[1.0.0] - 2024-01-15
✨ Ajoutés
  • Création de decks Pauper
  • Validation automatique des règles
```

---

## Support des versions

| Version | Statut | Support jusqu'au |
|---------|--------|------------------|
| 2.x.x | 🔮 Planifiée | TBD |
| 1.1.x | 📆 Prochainement | 2024-12-31 |
| **1.0.x** | ✅ **Actuelle** | 2025-12-31 |
| 0.9.x | ⛔ Obsolète | 2024-02-28 |

---

## Signaler un bug

Si vous découvrez un bug:

1. Vérifiez s'il existe déjà une Issue
2. Créez une nouvelle Issue avec:
   - Description claire du bug
   - Étapes de reproduction
   - Version affectée
   - Environnement (OS, navigateur)

3. Consultez [CONTRIBUTING.md](CONTRIBUTING.md)

## Proposer une feature

1. Ouvrez une Discussion GitHub
2. Décrivez votre idée
3. Attendez les retours
4. Si validée, deviendra une Issue

---

**Dernière mise à jour :** 2026
**Version documentée:** 1.0.0  
**Responsable:** Jérôme Guillevic
