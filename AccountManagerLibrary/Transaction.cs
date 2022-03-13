using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager
{
    public class Transaction
    {
        #region Constructors

        public Transaction(string description, DateTime date, decimal amount)
        {
            Description = description;
            Date = date;
            Amount = amount;
        }

        #endregion Constructors

        #region Properties

        public decimal Amount { get; }
        public List<Category> Categories { get; } = new List<Category>();
        public DateTime Date { get; }
        public string Description { get; }

        #endregion Properties
    }
}