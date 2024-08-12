namespace AccountManagerCore
{
    public static class AccountManager
    {
        private static readonly List<Account> accounts = [];
        private static readonly List<TypicalTransaction> typicalTransactions = [];

        public static IEnumerable<Account> GetAccounts() => new List<Account>(accounts);
        public static IEnumerable<TypicalTransaction> GetTypicalTransactions() => new List<TypicalTransaction>(typicalTransactions);
    }
}
