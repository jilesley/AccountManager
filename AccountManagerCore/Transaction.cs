namespace AccountManagerCore
{
    public class Transaction
    {
        internal Transaction(string description, DateTime date, decimal amount, Account account)
        {
            Description = description;
            Date = date;
            Amount = amount;
            Account = account;
        }

        public string Description { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public Account Account { get; }
        public string? HumanDescription { get; set; }
    }
}
