using System.Security.Claims;
using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController(AppDbContext context) 
    : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost]
    [Route("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        var user = new Account
        {
            Email = request.Email,
            Username = request.Username,
            Password = request.Password
        };
        
        await _context.Accounts.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return Ok(new
        {
            message = "Account created successfully"
        });
    }
    
    [HttpPost]
    [Route("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var user = await _context.Accounts.FirstOrDefaultAsync(user => user.Username == request.Username);
        if (user is null) 
            return NotFound();
        
        if (user.Password != request.Password)
            return Unauthorized();

        return SignIn(
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim("sub", Guid.NewGuid().ToString()),
                    ],
                    BearerTokenDefaults.AuthenticationScheme
                )
            ),
            authenticationScheme: BearerTokenDefaults.AuthenticationScheme
        );
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var accounts = await _context.Accounts.ToListAsync();
        return Ok(accounts);
    }
}

public record SignUpRequest(string Email, string Username, string Password);
public record SignInRequest(string Username, string Password);