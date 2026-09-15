using Microsoft.EntityFrameworkCore;
using SpelloggenApi.Models;

namespace SpelloggenApi.Data;

public class SpelloggenContext : DbContext
{
    public SpelloggenContext(DbContextOptions<SpelloggenContext> options) : base(options) { }

    public DbSet<Spel> Spel => Set<Spel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seeded with HasData so EnsureCreated() writes these rows when it creates the file.
        // All six are live service games, which is why none of them has the status "Slutat".
        // Teamfight Tactics and FIFA 25 have no Rank, and FIFA 25 has no cover image.
        // That is on purpose: it proves the nullable fields and the image placeholder
        // do not break the UI.
        modelBuilder.Entity<Spel>().HasData(
            new Spel
            {
                Id = 1,
                Titel = "PUBG: Battlegrounds",
                Plattform = "PC",
                Status = "Spelar aktivt",
                Rank = "Diamond 2",
                SpeladeTimmar = 2420,
                SenastSpelad = new DateTime(2026, 9, 14),
                Anteckningar = "Spelar mest squad med samma gäng varje kväll.",
                BildUrl = "/uploads/pubg.svg"
            },
            new Spel
            {
                Id = 2,
                Titel = "Dota 2",
                Plattform = "PC",
                Status = "Spelar aktivt",
                Rank = "3800 MMR",
                SpeladeTimmar = 2274,
                SenastSpelad = new DateTime(2026, 9, 13),
                Anteckningar = "Position 4 support. Siktar på Divine den här säsongen.",
                BildUrl = "/uploads/dota2.svg"
            },
            new Spel
            {
                Id = 3,
                Titel = "Counter-Strike 2",
                Plattform = "PC",
                Status = "Spelar aktivt",
                Rank = "Global Elite",
                SpeladeTimmar = 3224,
                SenastSpelad = new DateTime(2026, 9, 15),
                Anteckningar = "Mest premier, lite faceit på helgerna.",
                BildUrl = "/uploads/cs2.svg"
            },
            new Spel
            {
                Id = 4,
                Titel = "League of Legends",
                Plattform = "PC",
                Status = "Pausad",
                Rank = "Platinum 4",
                SpeladeTimmar = 2050,
                SenastSpelad = new DateTime(2026, 6, 2),
                Anteckningar = "Pausad sedan i somras. Kommer nog tillbaka till preseason.",
                BildUrl = "/uploads/lol.svg"
            },
            new Spel
            {
                Id = 5,
                Titel = "Teamfight Tactics",
                Plattform = "PC",
                Status = "Spelar aktivt",
                Rank = null,
                SpeladeTimmar = 160,
                SenastSpelad = new DateTime(2026, 9, 10),
                Anteckningar = "Spelar bara normals, har aldrig brytt mig om ranked här.",
                BildUrl = "/uploads/tft.svg"
            },
            new Spel
            {
                Id = 6,
                Titel = "FIFA 25",
                Plattform = "PC",
                Status = "Pausad",
                Rank = null,
                SpeladeTimmar = 45,
                SenastSpelad = new DateTime(2026, 4, 18),
                Anteckningar = "Köptes på rea, blev aldrig av att spela så mycket.",
                BildUrl = null
            }
        );
    }
}
