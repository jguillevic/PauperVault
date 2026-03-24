# 🚀 Guide de Déploiement

Guide complet pour déployer **PauperVault** en production.

## 📋 Table des matières

1. [Pré-déploiement](#pré-déploiement)
2. [Stratégies de déploiement](#stratégies-de-déploiement)
3. [Déploiement sur Azure](#déploiement-sur-azure)
4. [Déploiement sur serveur on-premise](#déploiement-sur-serveur-on-premise)
5. [Déploiement Docker](#déploiement-docker)
6. [Post-déploiement](#post-déploiement)
7. [Rollback](#rollback)
8. [Monitoring](#monitoring)

## ✅ Pré-déploiement

### Checklist de déploiement

#### Code Quality
- [ ] Tous les tests passent : `dotnet test`
- [ ] Pas d'avertissements (warnings) : `dotnet build -c Release /p:TreatWarningsAsErrors=true`
- [ ] Code review complètement approuvé
- [ ] Pas de code mort ou temporaire
- [ ] Commentaires TODO/FIXME supprimés

#### Documentation
- [ ] README.md à jour
- [ ] CHANGELOG.md mise à jour avec la version
- [ ] Comentaires XML sur les méthodes publiques
- [ ] Guide de migration si breaking changes

#### Configuration
- [ ] Variables d'environnement correctes
- [ ] Secrets sécurisés (pas en dur dans le code)
- [ ] Fichiers de configuration en place
- [ ] Certificats SSL valides

#### Données
- [ ] Backup de la BD de production
- [ ] Plan de rollback préparé
- [ ] Migrations testées en staging
- [ ] Données de seed en place si nécessaire

#### Performance & Sécurité
- [ ] Tests de charge exécutés
- [ ] Scan de sécurité SAST complété
- [ ] Dépendances à jour (dotnet list package --outdated)
- [ ] Vulnérabilités connues vérifiées

#### Infrastructure
- [ ] Instances disponibles
- [ ] Espace disque suffisant
- [ ] Bande passante suffisante
- [ ] Load balancer configuré

### Version Planning

```
Release: v1.2.0
===============

Nommage des branches:
- main              Production (stable)
- staging           Pré-production (test final)
- develop           Développement
- feature/xxx       Feature en cours
- bugfix/xxx        Correction en cours

Timeline:
- 2024-02-01 : Code freeze
- 2024-02-05 : Test complet
- 2024-02-08 : Déploiement staging
- 2024-02-10 : Approbation final
- 2024-02-12 : Déploiement production
```

## 🎯 Stratégies de déploiement

### 1. Blue-Green Deployment

```
┌─────────────────┐
│ Load Balancer   │
└────────┬────────┘
         │
    ┌────┴────┐
    │          │
┌───▼──┐   ┌──▼────┐
│ Blue │   │ Green │
│ v1.1 │   │ v1.2  │
└──────┘   └───────┘

Étape 1: Déployer v1.2 sur Green
Étape 2: Tester Green
Étape 3: Basculer le traffic vers Green
Étape 4: Garder Blue comme rollback
```

**Avantages:**
- ✅ Zéro downtime
- ✅ Rollback immédiat
- ✅ Test en production

**Inconvénients:**
- ❌ Double infrastructure
- ❌ Coût plus élevé

### 2. Rolling Deployment

```
Instance 1: v1.1 → v1.2 ✓
Instance 2: v1.1 → v1.2 ✓
Instance 3: v1.1 → v1.2 ✓

Tous restent actifs pendant la migration
```

**Avantages:**
- ✅ Infrastructure minimale
- ✅ Moins de coûts

**Inconvénients:**
- ❌ Downtime court possible
- ❌ Gestion compliquée de deux versions

### 3. Canary Deployment

```
Version v1.2 déployée pour:
- 10% du trafic (Jour 1)
- 25% du trafic (Jour 2)
- 50% du trafic (Jour 3)
- 100% du trafic (Jour 4)

Monitoring continu, rollback si problème
```

**Avantages:**
- ✅ Risque limité
- ✅ Feedback utilisateur progressif

**Inconvénients:**
- ❌ Déploiement plus long
- ❌ Complexité augmentée

### Recommandation

Pour PauperVault: **Blue-Green** ou **Canary**

## ☁️ Déploiement sur Azure

### Configuration initiale

```bash
# 1. Créer un groupe de ressources
az group create --name paupervault-rg --location eastus

# 2. Créer un App Service Plan
az appservice plan create \
  --name paupervault-plan \
  --resource-group paupervault-rg \
  --sku B2 \
  --is-linux

# 3. Créer l'App Service
az webapp create \
  --resource-group paupervault-rg \
  --plan paupervault-plan \
  --name paupervault-prod \
  --runtime "DOTNET|9.0"

# 4. Créer la BD SQL
az sql server create \
  --name paupervault-sql \
  --resource-group paupervault-rg \
  --admin-user admin \
  --admin-password "ComplexPassword123!"

# 5. Créer la base de données
az sql db create \
  --resource-group paupervault-rg \
  --server paupervault-sql \
  --name PauperVaultDb
```

### Déploiement avec GitHub Actions

Créer `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build -c Release --no-restore
    
    - name: Run tests
      run: dotnet test -c Release --no-build
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: paupervault-prod
        publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
        package: ./publish
    
    - name: Run migrations
      run: |
        az webapp config appsettings set \
          --name paupervault-prod \
          --resource-group paupervault-rg \
          --settings ASPNETCORE_ENVIRONMENT=Production
```

## 🐳 Déploiement Docker

### Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["PauperVault.Api/PauperVault.Api.csproj", "PauperVault.Api/"]
COPY ["PauperVault.Web/PauperVault.Web.csproj", "PauperVault.Web/"]
COPY ["PauperVault.Core/PauperVault.Core.csproj", "PauperVault.Core/"]
COPY ["PauperVault.Contracts/PauperVault.Contracts.csproj", "PauperVault.Contracts/"]

RUN dotnet restore "PauperVault.Api/PauperVault.Api.csproj"

COPY . .
RUN dotnet build "PauperVault.Api/PauperVault.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PauperVault.Api/PauperVault.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80 443

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "PauperVault.Api.dll"]
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  sql-server:
    image: mcr.microsoft.com/mssql/server:latest
    environment:
      SA_PASSWORD: "YourPassword123!"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

  app:
    build: .
    ports:
      - "80:80"
      - "443:443"
    environment:
      ConnectionStrings__DefaultConnection: "Server=sql-server;Database=PauperVaultDb;User Id=sa;Password=YourPassword123!"
      ASPNETCORE_ENVIRONMENT: "Production"
    depends_on:
      - sql-server
    volumes:
      - app_data:/app/data

volumes:
  sqlserver_data:
  app_data:
```

### Déployer avec Docker

```bash
# Build l'image
docker build -t paupervault:1.2.0 .

# Démarrer les conteneurs
docker-compose up -d

# Voir les logs
docker-compose logs -f app

# Arrêter
docker-compose down
```

## 🖥️ Déploiement on-premise

### Serveur Windows

```powershell
# 1. Préparer le serveur
# - Installer .NET 9 Runtime
# - Installer SQL Server
# - Configurer IIS
# - Configurer HTTPS

# 2. Publier l'application
dotnet publish -c Release -o C:\PauperVault\release

# 3. Créer un site IIS
$iisParams = @{
    Name = 'PauperVault'
    PhysicalPath = 'C:\PauperVault\release'
    BindingInformation = '*:443:paupervault.com'
    Protocol = 'https'
}
New-IISSite @iisParams

# 4. Configurer l'Application Pool
$appPool = New-WebAppPool -Name 'PauperVault'
$appPool.ManagedRuntimeVersion = 'v4.0'
$appPool.AutoStart = $true

# 5. Appliquer les migrations
cd C:\PauperVault\release
dotnet PauperVault.Api.dll --migrate

# 6. Redémarrer IIS
Restart-WebAppPool -Name 'PauperVault'
```

### Serveur Linux

```bash
#!/bin/bash

# 1. Préparer le serveur
sudo apt-get update
sudo apt-get install -y dotnet-runtime-9.0 nginx

# 2. Créer l'utilisateur d'application
sudo useradd -m -s /bin/bash paupervault

# 3. Publier l'application
cd /home/paupervault
dotnet publish -c Release -o ./app

# 4. Configurer Nginx comme reverse proxy
sudo tee /etc/nginx/sites-available/paupervault > /dev/null <<EOF
server {
    listen 80;
    server_name paupervault.com;
    
    location / {
        proxy_pass http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
    }
}
EOF

# 5. Activer le site
sudo ln -s /etc/nginx/sites-available/paupervault /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx

# 6. Créer un service systemd
sudo tee /etc/systemd/system/paupervault.service > /dev/null <<EOF
[Unit]
Description=PauperVault Web Application
After=network.target

[Service]
Type=notify
User=paupervault
WorkingDirectory=/home/paupervault/app
ExecStart=/usr/bin/dotnet /home/paupervault/app/PauperVault.Api.dll
Restart=on-failure
RestartSec=10

[Install]
WantedBy=multi-user.target
EOF

# 7. Démarrer le service
sudo systemctl daemon-reload
sudo systemctl enable paupervault
sudo systemctl start paupervault
```

## 📤 Post-déploiement

### Vérification initiale

```bash
# 1. Vérifier la santé
curl https://paupervault.com/health

# 2. Vérifier l'API
curl https://paupervault.com/api/cards?page=1

# 3. Vérifier les logs
# Azure: az webapp log tail
# Docker: docker-compose logs -f
# Systemd: journalctl -u paupervault -f

# 4. Vérifier les perf
# - Lighthouse score
# - Page load time
# - API response time
```

### Smoke tests

```csharp
[Test]
public async Task DeploymentHealthCheck()
{
    using var client = new HttpClient();
    var response = await client.GetAsync("https://paupervault.com/health");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}

[Test]
public async Task ApiIsAccessible()
{
    using var client = new HttpClient();
    var response = await client.GetAsync("https://paupervault.com/api/decks?page=1");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}
```

### Configuration monitoring

```csharp
// Program.cs
builder.Services.AddApplicationInsights();

using var telemetryClient = new TelemetryClient();
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();
    
    telemetryClient.TrackEvent("RequestCompleted", new Dictionary<string, string>
    {
        { "Path", context.Request.Path },
        { "Duration", stopwatch.ElapsedMilliseconds.ToString() }
    });
});
```

## ⏮️ Rollback

### Procédure de rollback

```bash
# 1. Identifier le problème
# Vérifier les logs
# Analyser les erreurs

# 2. Décider du rollback
# Si problème critique → rollback immédiat
# Si problème mineur → corriger et redéployer

# 3. Exécuter le rollback

# Option A: Blue-Green
# Basculer le load balancer vers Blue

# Option B: Docker
docker-compose down
git checkout v1.1.0
docker-compose up -d

# Option C: Azure
az webapp deployment slot swap \
  --resource-group paupervault-rg \
  --name paupervault-prod \
  --slot staging

# 4. Vérifier la stabilité
curl https://paupervault.com/health

# 5. Analyser la cause
# Post-mortem
# Leçons apprises
```

## 📊 Monitoring

### Métriques clés

```
Performance
├─ Uptime: > 99.95%
├─ Response time: < 500ms (p95)
├─ Error rate: < 0.1%
├─ CPU usage: < 80%
├─ Memory usage: < 500MB
└─ Disk usage: < 80%

Business
├─ Decks created (daily)
├─ API calls (daily)
├─ Active users
└─ Errors reported

Security
├─ Failed login attempts
├─ Unauthorized accesses
├─ Malicious patterns
└─ Security scan results
```

### Configuration d'alertes

```
Uptime monitoring
└─ Alert if response time > 1000ms
└─ Alert if status code != 200

Error monitoring
└─ Alert if error rate > 1%
└─ Alert if exception count > 100/hour

Resource monitoring
└─ Alert if CPU > 85%
└─ Alert if memory > 600MB
└─ Alert if disk > 90%

Security monitoring
└─ Alert if failed logins > 10/minute
└─ Alert if SQL injection detected
└─ Alert if DDoS pattern detected
```

---

**Dernière mise à jour :** 2026
**Version :** 1.0  
**Responsable :** Jérôme Guillevic

Pour une assistance, consultez [CONTRIBUTING.md](CONTRIBUTING.md)
