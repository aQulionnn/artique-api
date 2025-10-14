using Artique.Api.Data.Configurations;
using Artique.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Artwork> Artworks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new AccountConfiguration())
            .ApplyConfiguration(new ArtistConfiguration())
            .ApplyConfiguration(new ArtworkConfiguration());
    }
}