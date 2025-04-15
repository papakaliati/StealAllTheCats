public class CatEntity
{
    public int Id { get; set; }
    public string CatId { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Image { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public List<CatTag> CatTags { get; set; } = [];
}