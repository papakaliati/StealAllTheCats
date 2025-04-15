namespace StealAllTheCats.Integrations.TheCatApi.Models
{
    public class BaseCatApiResponse
    {
        public required string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public required string Url { get; set; }
    }

    public class CatApiResponse: BaseCatApiResponse
    {
        public required List<Breed> Breeds { get; set; }
    }

    public class Breed
    {
        public required string Temperament { get; set; }
    }
}
