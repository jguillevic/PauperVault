using PauperVault.Web.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

builder.AddPauperVaultServices();

var app = builder.Build();

app.UsePauperVaultPipeline();

app.Run();
