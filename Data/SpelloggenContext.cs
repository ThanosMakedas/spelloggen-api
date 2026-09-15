using Microsoft.EntityFrameworkCore;
using SpelloggenApi.Models;

namespace SpelloggenApi.Data;

public class SpelloggenContext : DbContext
{
    public SpelloggenContext(DbContextOptions<SpelloggenContext> options) : base(options) { }

    public DbSet<Spel> Spel => Set<Spel>();
}
