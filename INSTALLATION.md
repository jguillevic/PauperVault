# 🚀 Guide d'Installation et Configuration

Guide complet pour installer et configurer **PauperVault** en local.

## 📋 Prérequis

### Système d'exploitation
- **Windows 10/11** (ou Windows Server 2019+)
- **macOS 11+** (Intel ou Apple Silicon)
- **Linux** (Ubuntu 20.04+, CentOS 8+, Debian 10+)

### Logiciels requis

| Logiciel | Version | Lien |
|----------|---------|------|
| .NET SDK | 9.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| Visual Studio | 2026+ | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/downloads/) |
| SQL Server | 2019+ ou LocalDB | [sqlserver-download](https://www.microsoft.com/sql-server) |
| Git | 2.30+ | [git-scm.com](https://git-scm.com/) |
| Node.js | 18+ (optionnel) | [nodejs.org](https://nodejs.org/) |

### Espace disque

- **Minimum :** 2 GB
- **Recommandé :** 5+ GB (avec toutes les dépendances)
- **Base de données :** 500 MB (extensible)

## ⚙️ Installation pas à pas

### 1. Vérifier les prérequis

```bash
# Vérifier .NET
dotnet --version

# Vérifier Git
git --version

# Vérifier SQL Server (Windows)
sqlcmd -S (local)\mssqlserver -Q "SELECT @@VERSION"
```

### 2. Cloner le repository

#### Avec SSH (recommandé)

```bash
# Configurer SSH (première fois)
ssh-keygen -t ed25519 -C "votre-email@example.com"

# Ajouter la clé SSH à GitHub
# https://github.com/settings/ssh/new

# Cloner avec SSH
git clone git@github.com:jguillevic/PauperVault.git
cd PauperVault
```

#### Avec HTTPS

```bash
git clone https://github.com/jguillevic/PauperVault.git
cd PauperVault
```

### 3. Restaurer les dépendances

```bash
# Restaurer les packages NuGet
dotnet restore

# Vérifier que tout est bien restauré
dotnet build --no-restore
```

### 4. Configurer la base de données

#### Option A : Utiliser LocalDB (simplifié, Windows uniquement)

```bash
# LocalDB s'installe avec Visual Studio

# Créer la BD
dotnet ef database update --project PauperVault.Api

# Vérifier la connexion
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"
```

#### Option B : Utiliser SQL Server Express

```bash
# Installer SQL Server Express
# https://www.microsoft.com/sql-server/sql-server-2022-express

# Mettre à jour la connection string
# Voir la section "Configuration"

# Créer la BD
dotnet ef database update --project PauperVault.Api
```

#### Option C : Utiliser Docker (recommandé)

```bash
# Démarrer SQL Server dans Docker
docker run -e "ACCEPT_EULA=Y" \
           -e "SA_PASSWORD=YourPassword123!" \
           -p 1433:1433 \
           -d mcr.microsoft.com/mssql/server:latest

# Mettre à jour appsettings.json
# Voir la section "Configuration"

# Créer la BD
dotnet ef database update --project PauperVault.Api
```

### 5. Seeder les données (optionnel)

```bash
# Ajouter des données de test
dotnet run --project PauperVault.Api --seed

# Ou manuellement
# Voir PauperVault.Api\Infrastructure\Seeding\
```

## 🔧 Configuration

### Configuration par fichier

#### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PauperVaultDb;Trusted_Connection=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
    }
  },
  "Jwt": {
    "SecretKey": "votre-cle-secrete-tres-longue-128-caracteres",
    "Issuer": "PauperVault",
    "Audience": "PauperVaultUsers",
    "ExpirationMinutes": 1440
  },
  "AllowedHosts": "*"
}
```

#### `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:7001"]
  }
}
```

#### `appsettings.Production.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server.database.windows.net;Database=PauperVaultDb;User Id=admin;Password=votre-password-securise;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Jwt": {
    "SecretKey": "utilisez-un-secret-manager-en-production",
    "ExpirationMinutes": 60
  }
}
```

### Variables d'environnement

```powershell
# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "votre-string-de-connexion"
$env:Jwt__SecretKey = "votre-cle-secrete"

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="votre-string-de-connexion"
export Jwt__SecretKey="votre-cle-secrete"
```

### Secrets (développement local)

```bash
# Initialiser le gestionnaire de secrets
dotnet user-secrets init --project PauperVault.Api

# Ajouter un secret
dotnet user-secrets set "Jwt:SecretKey" "votre-cle-secrete" --project PauperVault.Api

# Lister les secrets
dotnet user-secrets list --project PauperVault.Api

# Supprimer un secret
dotnet user-secrets remove "Jwt:SecretKey" --project PauperVault.Api
```

## ▶️ Lancer l'application

### Méthode 1 : Ligne de commande

```bash
# Démarrer l'application
dotnet run --project PauperVault.Api

# Accéder à l'application
# https://localhost:7001
# http://localhost:5001
```

### Méthode 2 : Visual Studio

1. Ouvrir `PauperVault.sln` dans Visual Studio
2. Configurer le projet de démarrage : `PauperVault.Api`
3. Appuyer sur `F5` ou cliquer "Start"

### Méthode 3 : Visual Studio Code

```bash
# Ouvrir le workspace
code .

# Installer l'extension C#
# Installer l'extension REST Client (optionnel)

# Lancer le debugger : F5
```

### Première visite

```
URL : https://localhost:7001

Options :
- /swagger          Swagger UI (documentation API)
- /health           Health check
- /decks            Page d'accueil (Razor Page)
```

## 🧪 Tester l'installation

### Health Check

```bash
curl https://localhost:7001/health
```

### Tester la BD

```bash
# Ouvrir une requête
curl https://localhost:7001/api/cards?page=1

# Ou via Swagger
# https://localhost:7001/swagger
```

### Exécuter les tests

```bash
# Tous les tests
dotnet test

# Tests spécifiques
dotnet test --filter "Category=Integration"

# Avec couverture
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## 🐛 Troubleshooting

### Problème : Connection string invalide

```
error: Cannot connect to database
```

**Solution:**
```bash
# Vérifier la connexion
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT 1"

# Recréer la BD
dotnet ef database drop --project PauperVault.Api -f
dotnet ef database update --project PauperVault.Api
```

### Problème : Certificat HTTPS invalide

```
error: The certificate is invalid
```

**Solution (développement uniquement):**
```bash
# Générer un certificat auto-signé
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Problème : Port déjà utilisé

```
error: Address already in use
```

**Solution:**
```powershell
# Windows - Trouver le processus
netstat -ano | findstr :7001
taskkill /PID <PID> /F

# Linux/macOS
lsof -i :7001
kill -9 <PID>
```

### Problème : Migrations manquantes

```
error: Database schema version mismatch
```

**Solution:**
```bash
# Appliquer les migrations
dotnet ef database update --project PauperVault.Api

# Ou créer une nouvelle migration
dotnet ef migrations add MigrationName --project PauperVault.Api
```

### Problème : Dépendances non restaurées

```
error: Project dependency not found
```

**Solution:**
```bash
# Nettoyer et restaurer
dotnet clean
dotnet restore

# Reconstruire
dotnet build
```

## 🔐 Sécurité en développement

### Certificat HTTPS auto-signé

```bash
# Générer
dotnet dev-certs https

# Afficher les détails
dotnet dev-certs https --check

# Truiter le certificat (macOS/Linux)
dotnet dev-certs https --trust
```

### Gestion des secrets sensibles

```bash
# JAMAIS en dur dans le code
# ❌ MAUVAIS
var connectionString = "Server=prod;Password=MyPassword123";

# ✅ BON - Utiliser Secret Manager
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
```

### CORS Configuration

```csharp
// Dans Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

## 📦 Installation des dépendances front-end (optionnel)

```bash
# Installer Node.js depuis nodejs.org

# Installer les packages
npm install

# Compiler les assets
npm run build

# Mode développement avec watch
npm run dev
```

## 🚀 Démarrage recommandé

```bash
# 1. Cloner
git clone https://github.com/jguillevic/PauperVault.git
cd PauperVault

# 2. Restaurer
dotnet restore

# 3. Secrets
dotnet user-secrets init --project PauperVault.Api
dotnet user-secrets set "Jwt:SecretKey" "dev-key-128-characters-minimum-needed" --project PauperVault.Api

# 4. BD
dotnet ef database update --project PauperVault.Api

# 5. Lancer
dotnet run --project PauperVault.Api

# 6. Accéder à
# https://localhost:7001
```

## 📚 Prochaines étapes

1. Lire [START_HERE.md](START_HERE.md)
2. Consulter [ARCHITECTURE.md](ARCHITECTURE.md)
3. Explorer le code dans les features
4. Lire les commentaires XML des classes

## 🆘 Support

- 📖 Documentation : Voir les fichiers `.md` du projet
- 🐛 Bug : Ouvrir une Issue GitHub
- 💬 Question : Ouvrir une Discussion GitHub
- 📧 Email : Contacter le mainteneur

---

**Dernière mise à jour :** 2026  
**Testé sur :** Windows 10/11, Ubuntu 20.04+, macOS 12+  
**Version .NET :** 9.0+
