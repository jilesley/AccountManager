using System;
using System.Collections.Generic;
using System.Linq;
using AccountManager;
using Xunit;

namespace AccountManager.Library.UnitTest
{
    public class DatabaseManagerCan
    {
        #region Fields

        private readonly string TestDatabasePath = @"D:\_development folder\AccountManager\AccountManagerLibraryUnitTest\TestDatabase.xml";

        private Account testAccount;

        #endregion Fields

        #region Methods

        [Fact(DisplayName = "Parse a database correctly")]
        public void ParseADatabaseCorrectly()
        {
            IEnumerable<Account> actual = DatabaseManager.ParseDatabase(TestDatabasePath);

            Account expected = GetTestAccount();


            // Test Valid IEnumerable
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);

            // Test Account contents
            Account account = actual.First();

            Assert.True(account.Name == expected.Name);
            Assert.True(account.Balance == expected.Balance);

            // Test Category contents
            Assert.NotNull(account.Categories);
            Assert.NotEmpty(account.Categories);

            Category actCategory = account.Categories.First();
            Category expCategory = expected.Categories.First();

            Assert.Equal(expCategory.Name, actCategory.Name);
            Assert.Contains(actCategory, actCategory.RequiredCategories);

            // Test Transaction contents
            Assert.NotNull(account.Transactions);
            Assert.NotEmpty(account.Transactions);

            Transaction actTransaction = account.Transactions.First();
            Transaction expTransaction = expected.Transactions.First();

            Assert.Equal(expTransaction.Description, actTransaction.Description);
            Assert.Equal(expTransaction.Date, actTransaction.Date);
            Assert.Equal(expTransaction.Amount, actTransaction.Amount);
            Assert.Contains(actCategory, actTransaction.Categories);
        }

        private Account GetTestAccount()
        {
            if (testAccount == null)
            {
                testAccount = new();

                testAccount.Name = "Test Account 1";
                testAccount.Balance = 1234.56m;

                Category category = new();
                category.Name = "Category 1";
                category.RequiredCategories.Add(category);

                testAccount.Categories.Add(category);

                Transaction transaction = new("Transaction 1", new DateTime(1996, 11, 29), -100m);
                transaction.Categories.Add(category);

                testAccount.Transactions.Add(transaction);
            }

            return testAccount;
        }

        #endregion Methods
    }
}