using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Controllers;

[Route("api/artworks")]
[ApiController]
public class ArtworkController(WriteDbContext context) 
    : ControllerBase
{
    private readonly WriteDbContext _context = context;

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

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, UpdateArtworkRequest request)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        
        if (artwork is null)
            return NotFound("Artwork not found");
        
        artwork.Title = request.Title;
        artwork.Description = request.Description;
        artwork.ImageUrl = request.ImageUrl;
        artwork.Year = request.Year;
        
        await _context.SaveChangesAsync();
        
        return Ok("Artwork updated");
    }

    [HttpPatch]
    [Route("{id:guid}/description")]
    public async Task<IActionResult> UpdateDescription([FromRoute] Guid id, [FromBody] UpdateDescriptionRequest request)
    {
        var artwork = await _context.Artworks.FindAsync(id);   
        
        if (artwork is null)
            return NotFound("Artwork not found");
        
        artwork.Description = request.Description;
        await _context.SaveChangesAsync();
        
        return Ok("Artwork updated");
    }
    
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        
        if (artwork is null)
            return NotFound("Artwork not found");
        
        _context.Artworks.Remove(artwork);
        await _context.SaveChangesAsync();
            
        return Ok("Artwork deleted");
    }
}

public record AddArtworkRequest(string Title ,string Description, string ImageUrl, int Year, Guid ArtistId);
public record UpdateArtworkRequest(string Title, string Description, string ImageUrl, int Year);
public record UpdateDescriptionRequest(string Description);