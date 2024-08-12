using static AccountManagerCore.TypicalTransaction;

namespace AccountManagerCore
{
    public static class AccountManager
    {
        private static readonly List<Account> accounts = [];
        private static readonly List<TypicalTransaction> typicalTransactions = [];

        public static IEnumerable<Account> GetAccounts() => new List<Account>(accounts);

        public static Account AddAccount()
        {
            Account newAccount = new("New Account");

            accounts.Add(newAccount);

            return newAccount;
        }

        public static bool RemoveAccount(Account account) => accounts.Remove(account);


        public static IEnumerable<TypicalTransaction> GetTypicalTransactions() => new List<TypicalTransaction>(typicalTransactions);

        public static IEnumerable<TypicalTransaction> AddTypicalTransactions(IEnumerable<FindTypicalTransactionResult> findTypicalTransactionResults)
        {
            List<TypicalTransaction> updatedTypicalTransactions = [];

            foreach (FindTypicalTransactionResult result in findTypicalTransactionResults)
            {
                result.AddNewTransactionsToTypical();
                typicalTransactions.Add(result.TypicalTransaction);
                updatedTypicalTransactions.Add(result.TypicalTransaction);
            }

            return updatedTypicalTransactions;
        }

        public static bool RemoveTypicalTransaction(TypicalTransaction typicalTransaction)
        {
            if (typicalTransactions.Remove(typicalTransaction))
            {
                foreach (Transaction transaction in typicalTransaction.GetTransactions())
                {
                    transaction.TypicalTransaction = null;
                }

                return true;
            }

            return false;
        }
    }
}
