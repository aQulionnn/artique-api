using Artique.Api.Data;
using Artique.Api.Inputs;
using Artique.Api.Models;
using Artique.Api.Types;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType(typeof(Query))]
public class ArtworkQueries
{
    public async Task<ICollection<ArtworkType>> GetArtworks([Service] ReadDbContext context)
    {
        return await context.Artworks.Include(a => a.Artist)
            .Select(a => new ArtworkType
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                ImageUrl = a.ImageUrl,
                Year = a.Year,
                Artist = new ArtistShortType { Id = a.Artist.Id, Name = a.Artist.Name }
            }).ToListAsync();
    }

    public async Task<ArtworkType> GetArtworkById([Service] ReadDbContext context, Guid id)
    {
        var artwork = await context.Artworks.Include(a => a.Artist).FirstOrDefaultAsync(a => a.Id == id);
        if (artwork == null) throw new GraphQLException("Artwork not found");
        return new ArtworkType
        {
            Id = artwork.Id,
            Title = artwork.Title,
            Description = artwork.Description,
            ImageUrl = artwork.ImageUrl,
            Year = artwork.Year,
            Artist = new ArtistShortType { Id = artwork.Artist.Id, Name = artwork.Artist.Name }
        };
    }

    public async Task<ICollection<ArtworkShortType>> GetArtworksByArtistId([Service] ReadDbContext context,
        Guid artistId)
    {
        return await context.Artworks.Where(a => a.ArtistId == artistId)
            .Select(a => new ArtworkShortType
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                Year = a.Year
            }).ToListAsync();
    }

    public async Task<ICollection<ArtworkShortType>> SearchArtworks([Service] ReadDbContext context,
        SearchArtworksInput input)
    {
        var query = context.Artworks.AsQueryable();
        if (!string.IsNullOrWhiteSpace(input.Title))
            query = query.Where(a => a.Title.ToLower().Contains(input.Title.ToLower()));
        if (input.ArtistIds is { Count: > 0 }) query = query.Where(a => input.ArtistIds.Contains(a.ArtistId));
        if (input.Years is { Count: > 0 }) query = query.Where(a => input.Years.Contains(a.Year));

        return await query.Select(a => new ArtworkShortType
        {
            Id = a.Id,
            Title = a.Title,
            ImageUrl = a.ImageUrl,
            Year = a.Year
        }).ToListAsync();
    }
}