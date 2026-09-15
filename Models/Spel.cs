using System.ComponentModel.DataAnnotations;

namespace SpelloggenApi.Models;

/// <summary>
/// One game in the log. There is no game logic anywhere in this project,
/// this is a plain record of what the user plays.
/// </summary>
public class Spel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel is required")]
    [MaxLength(200)]
    public string Titel { get; set; } = "";

    /// <summary>"PC", "PS5" or "Switch".</summary>
    [MaxLength(50)]
    public string Plattform { get; set; } = "PC";

    /// <summary>"Spelar aktivt", "Pausad", "Slutat" or "Vill testa".</summary>
    [MaxLength(50)]
    public string Status { get; set; } = "Vill testa";

    /// <summary>
    /// Free text rank such as "Ascendant", "5200 MMR" or "Div 3".
    /// Nullable because not every game has a rank.
    /// </summary>
    [MaxLength(100)]
    public string? Rank { get; set; }

    public int SpeladeTimmar { get; set; }

    public DateTime SenastSpelad { get; set; }

    public string Anteckningar { get; set; } = "";

    /// <summary>
    /// Path to the cover image, relative to the API root, for example "/uploads/pubg.svg".
    /// Nullable because a game does not need a cover.
    /// </summary>
    [MaxLength(300)]
    public string? BildUrl { get; set; }
}
