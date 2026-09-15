using Microsoft.EntityFrameworkCore;
using SpelloggenApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<SpelloggenContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Create the SQLite file on first run so the only startup step is "dotnet run".
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SpelloggenContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

// No UseHttpsRedirection: it would force the reader to trust a dev certificate first.
// No UseAuthorization: this project has no auth.
app.MapControllers();

app.Run();
