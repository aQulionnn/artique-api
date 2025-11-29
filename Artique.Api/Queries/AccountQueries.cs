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

    public async Task<Account> GetAccountById([FromServices] AppDbContext context, Guid id)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

        if (account is null)
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Account not found")
                .SetCode("NOT_FOUND")
                .Build());

        return account;
    }

    public async Task<ICollection<Account>> SearchAccounts([FromServices] AppDbContext context, SearchAccountsInput input)
    {
        var accounts = await context.Accounts
            .Where(a => a.Email.ToLower().Contains(input.Text.ToLower()) || 
                        a.Username.ToLower().Contains(input.Text.ToLower()))
            .ToListAsync();
        
        return accounts;
    }
}

public sealed record SearchAccountsInput(string Text);