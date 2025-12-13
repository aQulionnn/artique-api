namespace Artique.Api.Types;

public class ArtworkType
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int Year { get; set; }
    public ArtistShortType Artist { get; set; }
}
