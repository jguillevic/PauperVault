using PauperVault.Web.Bootstrap;
using PauperVault.Web.Infrastructure.Http.PauperVault;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.AddPauperVaultServices();
builder.AddPauperVaultApi();

var app = builder.Build();

// Pipeline
app.UsePauperVaultPipeline();

app.Run();