namespace Artique.Api.Types;

public class ArtistType
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<ArtworkShortType> Artworks { get; set; }
}
