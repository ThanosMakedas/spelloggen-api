using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpelloggenApi.Data;
using SpelloggenApi.Models;

namespace SpelloggenApi.Controllers;

[ApiController]
[Route("api/spel")]
public class SpelController : ControllerBase
{
    private readonly SpelloggenContext _context;

    public SpelController(SpelloggenContext context)
    {
        _context = context;
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
    // The mobile app in December needs this for its detail screen.
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
}
