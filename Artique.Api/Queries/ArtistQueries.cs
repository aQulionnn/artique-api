using Artique.Api.Data;
using Artique.Api.Inputs;
using Artique.Api.Models;
using Artique.Api.Types;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType(typeof(Query))]
public class ArtistQueries
{
    public async Task<ICollection<ArtistType>> GetArtists([Service] AppDbContext context)
    {
        return await context.Artists.Include(a => a.Artworks)
            .Select(a => new ArtistType
            {
                Id = a.Id,
                Name = a.Name,
                Artworks = a.Artworks.Select(x => new ArtworkShortType
                {
                    Id = x.Id,
                    Title = x.Title,
                    ImageUrl = x.ImageUrl,
                    Year = x.Year
                }).ToList()
            }).ToListAsync();
    }

    public async Task<ArtistType> GetArtistById([Service] AppDbContext context, Guid id)
    {
        var artist = await context.Artists.Include(a => a.Artworks).FirstOrDefaultAsync(a => a.Id == id);
        if (artist == null) throw new GraphQLException("Artist not found");
        return new ArtistType
        {
            Id = artist.Id,
            Name = artist.Name,
            Artworks = artist.Artworks.Select(a => new ArtworkShortType
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                Year = a.Year
            }).ToList()
        };
    }

    public async Task<ICollection<ArtistShortType>> SearchArtists([Service] AppDbContext context, SearchArtistsInput input)
    {
        return await context.Artists.Where(a => a.Name.ToLower().Contains(input.Name.ToLower()))
            .Select(a => new ArtistShortType { Id = a.Id, Name = a.Name })
            .ToListAsync();
    }
}