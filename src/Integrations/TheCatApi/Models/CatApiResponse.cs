namespace StealAllTheCats.External.TheCatApi.Models
{
    public class BaseCatApiResponse
    {
        public string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
    }

    public class CatApiResponse: BaseCatApiResponse
    {
        public List<Breed> Breeds { get; set; }
    }

    public class Breed
    {
        public string Temperament { get; set; }
    }
}
