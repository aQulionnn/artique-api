using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Controllers;

[Route("api/artists")]
[ApiController]
public class ArtistController(AppDbContext context) 
    : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost]
    public async Task<IActionResult> Create(CreateArtistRequest request)
    {
        var artist = new Artist
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };
        
        await _context.Artists.AddAsync(artist);
        await _context.SaveChangesAsync();
        
        return Ok("Artist created");
    }
}

public record CreateArtistRequest(string Name);