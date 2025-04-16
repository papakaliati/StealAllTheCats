using System.ComponentModel.DataAnnotations;

public class CatEntity
{
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 3)]
    [Required]
    public string CatId { get; set; } = string.Empty;

    [Range(1, 10000)]
    public int Width { get; set; }

    [Range(1, 10000)]
    public int Height { get; set; }
    public string? Image { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public List<CatTag> CatTags { get; set; } = [];
}