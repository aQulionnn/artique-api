using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType(typeof(Query))]
public class ArtistQueries
{
    public async Task<ICollection<Artist>> GetArtists([Service] AppDbContext context)
    {
        var artists = await context.Artists
            .Include(a => a.Artworks)
            .ToListAsync();    
        
        return artists;
    }    
    
    public async Task<Artist> GetArtistById([Service] AppDbContext context, Guid id)
    {
        var artist = await context.Artists
            .Include(a => a.Artworks)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artist is null)
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Artist not found")
                .SetCode("NOT_FOUND")
                .Build());
        
        return artist;
    }
}