using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AccountManager;
using Xunit;

namespace AccountManager.Library.UnitTest
{
    public class DatabaseManagerCan
    {
        #region Fields

        private readonly string TestDatabasePath = @"D:\_development folder\AccountManager\AccountManagerLibraryUnitTest\TestDatabase.xml";
        private readonly string TempDatabasePath = @"C:\Users\James\Downloads\TempDatabase.xml";


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


        [Fact(DisplayName = "Update a database correctly")]
        public void UpdateADatabaseCorrectly()
        {
            DatabaseManager.UpdateDatabase(TempDatabasePath, 
                new List<Account> { GetTestAccount() });

            string actual = File.ReadAllText(TempDatabasePath);
            string expected = File.ReadAllText(TestDatabasePath);

            Assert.Equal(expected, actual);
        }


        [Fact(DisplayName = "Convert text into Transactions")]
        public void ConvertTextToTransactions()
        {
            string text =
@"01/12/2021,Nationwide Flex Savings HtBI BP,-200.00
01/12/2021,CHELM CITY COUNCIL DD,-168.00
26/11/2021,CUNDALL LTD CR,""1,911.04""";

            IEnumerable<Transaction> actual = DatabaseManager.ParseTransactions(text);

            IEnumerable<Transaction> expected = new List<Transaction>()
            {
                new Transaction(
                    "Nationwide Flex Savings HtBI BP",
                    new DateTime(2021, 12, 1),
                    -200.00m),
                new Transaction(
                    "CHELM CITY COUNCIL DD",
                    new DateTime(2021, 12, 1),
                    -168.00m),
                new Transaction(
                    "CUNDALL LTD CR",
                    new DateTime(2021, 11, 26),
                    1911.04m)
            };

            
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