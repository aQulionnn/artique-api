using Artique.Api.Data;
using Artique.Api.Inputs;
using Artique.Api.Models;
using Artique.Api.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artique.Api.Queries;

[ExtendObjectType<Query>]
public class AccountQueries
{
    public async Task<ICollection<AccountType>> GetAccounts([Service] AppDbContext context)
    {
        return await context.Accounts
            .Select(a => new AccountType
            {
                Id = a.Id,
                Email = a.Email,
                Username = a.Username
            }).ToListAsync();
    }

    public async Task<AccountType> GetAccountById([Service] AppDbContext context, Guid id)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        if (account == null) throw new GraphQLException("Account not found");
        return new AccountType
        {
            Id = account.Id,
            Email = account.Email,
            Username = account.Username
        };
    }

    public async Task<ICollection<AccountType>> SearchAccounts([Service] AppDbContext context, SearchAccountsInput input)
    {
        return await context.Accounts
            .Where(a => a.Email.ToLower().Contains(input.Text.ToLower()) ||
                        a.Username.ToLower().Contains(input.Text.ToLower()))
            .Select(a => new AccountType
            {
                Id = a.Id,
                Email = a.Email,
                Username = a.Username
            }).ToListAsync();
    }
}