namespace Artique.Api.Models;

public class Artist
{
    public Guid Id { get; init; }
    public string Name { get; set; }

    public ICollection<Artwork> Artworks { get; set; }
}