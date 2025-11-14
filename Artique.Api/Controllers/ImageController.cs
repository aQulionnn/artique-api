using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Artique.Api.Controllers;

[Route("api/images")]
[ApiController]
public class ImageController(Cloudinary cloudinary) 
    : ControllerBase
{
    private readonly Cloudinary _cloudinary = cloudinary;

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("No file was uploaded.");

        string uniqueFileName = Guid.NewGuid().ToString();

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(uniqueFileName, stream),
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };
        
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error == null) 
            return Ok(uploadResult.SecureUrl.ToString());

        return StatusCode(500, $"Upload failed: {uploadResult.Error.Message}");
    }
}