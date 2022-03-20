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
                Dictionary<Category, XElement> xRequiredCategories = new();
                int catId = 0;

                foreach (Category category in account.Categories)
                {
                    XElement categoryName = new XElement("Name", category.Name);
                    XElement requiredCategories = new XElement("RequiredCategories");

                    XElement xCategory = new XElement("Category",
                        categoryName, requiredCategories);
                    xCategory.SetAttributeValue("Id", catId);

                    categoryIds.Add(category, catId);
                    xRequiredCategories.Add(category, requiredCategories);

                    catId++;

                    xAccountCategories.Add(xCategory);
                }

                foreach (Category category in account.Categories)
                {
                    XElement requiredCategories = xRequiredCategories[category];

                    foreach (Category requiredCategory in category.RequiredCategories)
                    {
                        requiredCategories.Add(new XElement("Id", categoryIds[requiredCategory]));
                    }
                }


                foreach (Transaction transaction in account.Transactions)
                {
                    XElement xCategories = new XElement("Categories");
                    XElement xTransaction = new XElement("Transaction",
                        new XElement("Amount", transaction.Amount),
                        new XElement("Date",
                            new XElement("Day", transaction.Date.Day),
                            new XElement("Month", transaction.Date.Month),
                            new XElement("Year", transaction.Date.Year)),
                        new XElement("Description", transaction.Description),
                        xCategories);

                    xAccountTransactions.Add(xTransaction);

                    foreach (Category category in transaction.Categories)
                    {
                        xCategories.Add(new XElement("Id", categoryIds[category]));
                    }
                }
            }

            database.Save(path);
        }

        #endregion Methods
    }
}
