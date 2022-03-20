using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager
{
    public class Account
    {
        #region Properties

        public decimal Balance { get; set; }
        public List<Category> Categories { get; } = new List<Category>();
        public string Name { get; set; }
        public List<Transaction> Transactions { get; } = new List<Transaction>();

        #endregion Properties

        public override string ToString()
        {
            return Name;
        }
    }
}