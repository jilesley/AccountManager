namespace AccountManagerCore
{
    public class Account(string accountName)
    {
        private string name = accountName;
        private decimal initialBalance = 0;

        public string Name { get => name; set => name = value; }
        public decimal InitialBalance { get => initialBalance; set => initialBalance = value; }

        private readonly List<Transaction> transactions = [];


        public delegate void AccountTransactionsUpdatedEventHandler(object? sender, AccountTransactionsUpdatedEventArgs e);

        public event AccountTransactionsUpdatedEventHandler? TransactionsUpdated;


        private void RaiseTransactionsUpdated(Transaction transaction) => TransactionsUpdated?.Invoke(this, new(transaction));


        public IEnumerable<Transaction> GetTransactions() => new List<Transaction>(transactions);

        public IEnumerable<Transaction> GetTransactions(DateTime? startDate, DateTime? endDate)
        {
            return transactions.Where(t => t.Date <= startDate && t.Date >= endDate);
        }


        public Transaction AddTransaction(string description, DateTime date, decimal amount)
        {
            Transaction newTransaction = new(description, date, amount, this);

            transactions.Add(newTransaction);

            RaiseTransactionsUpdated(newTransaction);

            return newTransaction;
        }

        public decimal GetBalance() => initialBalance + transactions.Sum(t => t.Amount);
    }


    public class AccountTransactionsUpdatedEventArgs : EventArgs
    {
        public AccountTransactionsUpdatedEventArgs() { }
        public AccountTransactionsUpdatedEventArgs(Transaction transaction)
        {
            Transactions.Add(transaction);
        }
        public AccountTransactionsUpdatedEventArgs(IEnumerable<Transaction> transactions)
        {
            Transactions.AddRange(transactions);
        }
        List<Transaction> Transactions { get; } = [];
    }
}
