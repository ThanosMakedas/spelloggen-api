using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpelloggenApi.Data;
using SpelloggenApi.Models;

namespace SpelloggenApi.Controllers;

[ApiController]
[Route("api/spel")]
public class SpelController : ControllerBase
{
    private const long MaxBildStorlek = 5 * 1024 * 1024;
    private static readonly string[] TillatnaFilandelser = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly SpelloggenContext _context;
    private readonly IWebHostEnvironment _env;

    public SpelController(SpelloggenContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: /api/spel
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Spel>>> GetAlla()
    {
        return await _context.Spel
            .OrderByDescending(s => s.SenastSpelad)
            .ToListAsync();
    }

    // GET: /api/spel/{id}
    // The mobile app needs this for its detail screen.
    [HttpGet("{id}")]
    public async Task<ActionResult<Spel>> GetEtt(int id)
    {
        var spel = await _context.Spel.FindAsync(id);

        if (spel == null)
            return NotFound();

        return spel;
    }

    // POST: /api/spel
    [HttpPost]
    public async Task<ActionResult<Spel>> Skapa(Spel spel)
    {
        // Id is assigned by the database, ignore whatever the client sent.
        spel.Id = 0;

        _context.Spel.Add(spel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEtt), new { id = spel.Id }, spel);
    }

    // PUT: /api/spel/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Uppdatera(int id, Spel spel)
    {
        var befintligt = await _context.Spel.FindAsync(id);

        if (befintligt == null)
            return NotFound();

        befintligt.Titel = spel.Titel;
        befintligt.Plattform = spel.Plattform;
        befintligt.Status = spel.Status;
        befintligt.Rank = spel.Rank;
        befintligt.SpeladeTimmar = spel.SpeladeTimmar;
        befintligt.SenastSpelad = spel.SenastSpelad;
        befintligt.Anteckningar = spel.Anteckningar;

        // Keep the stored cover when the client does not send one, so editing
        // a game from the form cannot wipe an image that was uploaded earlier.
        if (spel.BildUrl != null)
            befintligt.BildUrl = spel.BildUrl;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: /api/spel/{id}/bild
    // Saves the file in wwwroot/uploads and stores its URL on the game.
    [HttpPost("{id}/bild")]
    public async Task<ActionResult<Spel>> LaddaUppBild(int id, IFormFile? fil)
    {
        var spel = await _context.Spel.FindAsync(id);

        if (spel == null)
            return NotFound();

        if (fil == null || fil.Length == 0)
            return BadRequest("Ingen fil skickades.");

        if (fil.Length > MaxBildStorlek)
            return BadRequest("Filen är större än 5 MB.");

        // No SVG here: it can contain scripts and would be served from our own origin.
        var filandelse = Path.GetExtension(fil.FileName).ToLowerInvariant();

        if (!TillatnaFilandelser.Contains(filandelse))
            return BadRequest("Endast .jpg, .jpeg, .png och .webp är tillåtna.");

        // A new random name, so two uploads never overwrite each other
        // and the file name from the client is never used as a path.
        var filnamn = $"{Guid.NewGuid()}{filandelse}";
        var mapp = Path.Combine(WebRoot, "uploads");
        Directory.CreateDirectory(mapp);

        using (var stream = System.IO.File.Create(Path.Combine(mapp, filnamn)))
        {
            await fil.CopyToAsync(stream);
        }

        var gammalBild = spel.BildUrl;
        spel.BildUrl = $"/uploads/{filnamn}";
        await _context.SaveChangesAsync();

        // Only remove the old image once the new one is saved.
        TaBortUppladdadBild(gammalBild);

        return spel;
    }

    // DELETE: /api/spel/{id}
    // Not required by the assignment, but it costs almost nothing to add.
    [HttpDelete("{id}")]
    public async Task<IActionResult> TaBort(int id)
    {
        var spel = await _context.Spel.FindAsync(id);

        if (spel == null)
            return NotFound();

        _context.Spel.Remove(spel);
        await _context.SaveChangesAsync();

        // Remove the image file too, so a deleted game does not leave a file behind.
        TaBortUppladdadBild(spel.BildUrl);

        return NoContent();
    }

    private string WebRoot => _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

    // Removes an image that was uploaded through the API.
    // The seeded covers are .svg files that belong to the repo, so those are kept.
    private void TaBortUppladdadBild(string? bildUrl)
    {
        if (string.IsNullOrEmpty(bildUrl) || !bildUrl.StartsWith("/uploads/"))
            return;

        if (bildUrl.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            return;

        var sokvag = Path.Combine(WebRoot, "uploads", Path.GetFileName(bildUrl));

        if (System.IO.File.Exists(sokvag))
            System.IO.File.Delete(sokvag);
    }
}
