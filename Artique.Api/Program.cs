using Artique.Api.Data;
using Artique.Api.Queries;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
    options.DisableStringReuse = true;
    options.Limits.MinRequestBodyDataRate = new MinDataRate(100,  TimeSpan.FromSeconds(10));
    options.Limits.MinResponseDataRate = new MinDataRate(100,  TimeSpan.FromSeconds(10));
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxConcurrentUpgradedConnections = 100;
    options.Limits.MaxRequestBodySize = 10 *  1024 * 1024;
});

Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken();

builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddDbContext<ReadDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddControllers();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<ArtworkQueries>()
    .AddTypeExtension<ArtistQueries>()
    .AddTypeExtension<AccountQueries>();

builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("Database")!,
        name: "ArtiqueDb",
        tags: ["db", "postgres", "neon"],
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
    var account = new Account(
        Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
        Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
        Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
    );

    return new Cloudinary(account);
});

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();

app.MapGraphQL("/graphql");
app.MapScalarApiReference("/scalar", options =>
{
    options.WithTitle("API");
    options.WithTheme(ScalarTheme.DeepSpace);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapControllers();

app.RunWithGraphQLCommands(args);

app.Run();