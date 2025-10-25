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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var artworks = await _context.Artworks
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Description,
                a.ImageUrl,
                a.Year,
                a.ArtistId,
            })
            .ToListAsync();
        
        return Ok(artworks);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var artwork = await _context.Artworks
            .Include(a => a.Artist)
            . FirstOrDefaultAsync(artwork => artwork.Id == id);
        
        if (artwork is null) return NotFound("Artwork not found");
        
        return Ok(new
        {
            artwork.Id,
            artwork.Title,
            artwork.Description,
            artwork.ImageUrl,
            artwork.Year,
            ArtistName = artwork.Artist.Name
        });
    }
}

public record AddArtworkRequest(string Title ,string Description, string ImageUrl, int Year, Guid ArtistId);