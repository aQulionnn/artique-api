namespace Artique.Api.Inputs;

public sealed record SearchArtworksInput(string? Title, ICollection<Guid>? ArtistIds, ICollection<int>? Years );