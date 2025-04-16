using System;
using System.ComponentModel.DataAnnotations;

public class TagEntity
{
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 3)]
    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public List<CatTag> CatTags { get; set; } = [];
}