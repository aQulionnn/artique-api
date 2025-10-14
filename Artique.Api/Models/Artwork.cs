namespace Artique.Api.Models;

public class Artwork
{
    public Guid Id { get; init; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int Year { get; set; }

    public Guid ArtistId { get; init; }
    public Artist Artist { get; set; }
}