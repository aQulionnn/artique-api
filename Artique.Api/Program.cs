using Artique.Api.Data;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "ArtiqueDb",
        tags: ["db", "postgres", "supabase"],
        failureStatus: HealthStatus.Unhealthy,
        timeout: TimeSpan.FromSeconds(3)
    );

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddSingleton(_ =>
{
    Env.Load();
    
    var account = new Account(
        Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
        Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
        Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
    );

    return new Cloudinary(account);
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("API");
    options.WithTheme(ScalarTheme.DeepSpace);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();