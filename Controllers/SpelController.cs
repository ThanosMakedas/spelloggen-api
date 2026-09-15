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
}
