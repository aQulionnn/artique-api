using Artique.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
}