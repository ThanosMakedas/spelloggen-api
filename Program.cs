using Microsoft.EntityFrameworkCore;
using SpelloggenApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<SpelloggenContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// The React dev server runs on a different port than the API, so the browser
// treats it as a different origin and blocks fetch without this policy.
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy => policy
        .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

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

app.UseCors("frontend");

// Serves wwwroot, which is where the cover images live.
// UseStaticFiles and not MapStaticAssets, because uploaded files do not exist at build time.
app.UseStaticFiles();

// No UseHttpsRedirection: it would force the reader to trust a dev certificate first.
// No UseAuthorization: this project has no auth.
app.MapControllers();

app.Run();
