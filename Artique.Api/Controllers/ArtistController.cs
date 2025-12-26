using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Controllers;

[Route("api/artists")]
[ApiController]
public class ArtistController(WriteDbContext context) 
    : ControllerBase
{
    private readonly WriteDbContext _context = context;

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

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, UpdateArtistRequest request)
    {
        var artist = await _context.Artists.FindAsync(id);
        
        if (artist is null)
            return NotFound("Artist not found");
        
        artist.Name = request.Name;
        await _context.SaveChangesAsync();
        
        return Ok("Artist updated");
    }
}

public record CreateArtistRequest(string Name);
public record UpdateArtistRequest(string Name);