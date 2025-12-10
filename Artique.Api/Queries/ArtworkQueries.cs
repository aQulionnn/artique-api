using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType(typeof(Query))]
public class ArtworkQueries
{
    public async Task<ICollection<Artwork>> GetArtworks([Service] AppDbContext context)
    {
        var artworks = await context.Artworks
            .Include(a => a.Artist)
            .ToListAsync();

        return artworks;
    }

    public async Task<Artwork> GetArtworkById([Service] AppDbContext context, Guid id)
    {
        var artwork = await context.Artworks
            .Include(a => a.Artist)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artwork is null)
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Artwork not found")
                .SetCode("NOT_FOUND")
                .Build());

        return artwork;
    }

    public async Task<ICollection<Artwork>> GetArtworksByArtistId([Service] AppDbContext context, Guid artistId)
    {
        var artworks = await context.Artworks
            .Where(a => a.ArtistId == artistId)
            .ToListAsync();
        
        return artworks;
    }

    public async Task<ICollection<Artwork>> SearchArtworks([Service] AppDbContext context, SearchArtworksInput input)
    {
        var query = context.Artworks.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(input.Title))
            query = query.Where(a => a.Title.ToLower().Contains(input.Title.ToLower()));

        if (input.ArtistIds is { Count: > 0 })
            query = query.Where(a => input.ArtistIds.Contains(a.ArtistId));
        
        if (input.Years is { Count: > 0 })
            query = query.Where(a => input.Years.Contains(a.Year));
        
        var artworks = await query.ToListAsync();
        
        return artworks;
    }
}

public sealed record SearchArtworksInput(string? Title, ICollection<Guid>? ArtistIds, ICollection<int>? Years );