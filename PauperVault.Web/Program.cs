using PauperVault.Web.Bootstrap;
using PauperVault.Web.Infrastructure.Http.PauperVault;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.AddPauperVaultServices();
builder.AddPauperVaultApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

// Pipeline
app.UsePauperVaultPipeline();

app.Run();