using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Controllers;

[Route("api/artworks")]
[ApiController]
public class ArtworkController(AppDbContext context) 
    : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost]
    public async Task<IActionResult> Add(AddArtworkRequest request)
    {
        var artwork = new Artwork
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Year = request.Year,
            ArtistId = request.ArtistId,
        };
        
        await _context.Artworks.AddAsync(artwork);
        await _context.SaveChangesAsync();
        
        return Ok("Artwork added");
    }
}

public record AddArtworkRequest(string Title ,string Description, string ImageUrl, int Year, Guid ArtistId);