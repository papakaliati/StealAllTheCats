public class CatTag
{
    public int CatEntityId { get; set; }
    public required CatEntity Cat { get; set; }
    public int TagEntityId { get; set; }
    public required TagEntity Tag { get; set; }
}