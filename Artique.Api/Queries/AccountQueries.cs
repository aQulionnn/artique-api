using Artique.Api.Data;
using Artique.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType<Query>]
public class AccountQueries
{
    public async Task<ICollection<Account>> GetAccounts([FromServices] AppDbContext context)
    {
        var accounts = await context.Accounts.ToListAsync();
        return accounts;
    }
}