using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PauperVault.Api.Features.Auth;
using PauperVault.Api.Features.Cards;
using PauperVault.Api.Features.Decks;
using PauperVault.Api.Infrastructure.Auth;
using PauperVault.Api.Infrastructure.Configuration;
using PauperVault.Api.Infrastructure.Data;
using PauperVault.Api.Infrastructure.Extensions;
using PauperVault.Api.Infrastructure.Scryfall;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationDatabases(builder.Configuration);
builder.Services.AddApplicationIdentity();
builder.Services.AddApplicationAuthentication(builder.Configuration);
builder.Services.AddApplicationHttpClients(builder.Configuration);

var app = builder.Build();

// Apply migrations
await MigrationService.ApplyMigrationsAsync(app.Services);

// Configure pipeline
app.UseApplicationPipeline();

await app.RunAsync();