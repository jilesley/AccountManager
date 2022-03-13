using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AccountManager
{
    public static class DatabaseManager
    {
        #region Methods

        public static IEnumerable<Account> ParseDatabase(string path)
        {
            List<Account> data = new();


            XDocument database = XDocument.Load(path);

            XElement Accounts = database.Root;

            foreach (XElement xAccount in Accounts.Elements())
            {
                Account account = new();

                account.Name = xAccount.Element("Name")?.Value;
                account.Balance = decimal.Parse(xAccount.Element("Balance").Value);


                Dictionary<int, Category> categoryIds = new Dictionary<int, Category>();

                foreach (XElement xCategory in xAccount.Element("Categories").Elements())
                {
                    Category category = new();

                    category.Name = xCategory.Element("Name")?.Value;
                    int id = int.Parse(xCategory.Attribute("Id")?.Value);
                    categoryIds.Add(id, category);

                    foreach (XElement xRequiredId in xCategory.Element("RequiredCategories").Elements())
                    {
                        int requiredId = int.Parse(xRequiredId?.Value);
                        category.RequiredCategories.Add(categoryIds[requiredId]);
                    }

                    account.Categories.Add(category);
                }


                foreach (XElement xTransaction in xAccount.Element("Transactions").Elements())
                {
                    string description = xTransaction.Element("Description")?.Value;
                    decimal amount = decimal.Parse(xTransaction.Element("Amount")?.Value);

                    XElement xDate = xTransaction.Element("Date");
                    DateTime date = new(
                        int.Parse(xDate.Element("Year")?.Value),
                        int.Parse(xDate.Element("Month")?.Value),
                        int.Parse(xDate.Element("Day")?.Value)
                    );

                    Transaction transaction = new(description, date, amount);

                    foreach (XElement xCategoryId in xTransaction.Element("Categories").Elements())
                    {
                        int categoryId = int.Parse(xCategoryId?.Value);
                        transaction.Categories.Add(categoryIds[categoryId]);
                    }

                    account.Transactions.Add(transaction);
                }


                data.Add(account);
            }

            return data;
        }

        public static void UpdateDatabase(string path, IEnumerable<Account> data)
        {
            XElement xAccounts = new("Accounts");
            XDocument database = new(xAccounts);

            foreach (Account account in data)
            {
                XElement xAccountCategories = new("Categories");
                XElement xAccountTransactions = new("Transactions");


                XElement xAccount = new("Account",
                    new XElement("Name", account.Name),
                    new XElement("Balance", account.Balance),
                    xAccountCategories,
                    xAccountTransactions
                );
                xAccounts.Add(xAccount);


                Dictionary<Category, int> categoryIds = new();
                int catId = 0;

                foreach (Category category in account.Categories)
                {
                }
            }

            database.Save(path);
        }

        #endregion Methods
    }
}