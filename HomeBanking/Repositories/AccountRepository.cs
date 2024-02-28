﻿using HomeBanking.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBanking.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public bool ExistsAccountNumber(string accountNumber)
        {
            return FindByCondition(account => account.Number == accountNumber).Any();
        }

        public Account FindById(long id)
        {
            return FindByCondition(account  => account.Id == id)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public Account FindByIdAndClientEmail(long id, string email)
        {
            return FindByCondition(account => account.Id == id && account.Client.Email == email)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions)
                .ToList();
        }

        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }
    }
}
