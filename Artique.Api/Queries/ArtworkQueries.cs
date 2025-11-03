using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType(typeof(Query))]
public class ArtworkQueries
{
    public async Task<ICollection<Artwork>> GetArtworks([Service] AppDbContext context)
    {
        var artworks = await context.Artworks.ToListAsync();    
        return artworks;
    } 
    
    public async Task<Artwork> GetArtworkById([Service] AppDbContext context, Guid id)
    {
        var artwork = await context.Artworks
            .Include(a => a.Artist)
            .FirstOrDefaultAsync(artwork => artwork.Id == id);

        if (artwork is null)
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Artwork not found")
                .SetCode("NOT_FOUND")
                .Build());
        
        return artwork;
    }
}