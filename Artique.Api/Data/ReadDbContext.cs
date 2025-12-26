using Artique.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Data;

public class ReadDbContext(DbContextOptions options) 
    : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Artwork> Artworks { get; set; }
}