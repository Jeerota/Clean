using Clean.Domain.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddServerSideBlazor();
builder.Services.SQLInfrastructure();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.Run();
