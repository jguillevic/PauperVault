using PauperVault.Web.Bootstrap;
using PauperVault.Web.Infrastructure.Http.PauperVault;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.AddPauperVaultServices();
builder.AddPauperVaultApi();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

app.UsePauperVaultPipeline();

app.Run();