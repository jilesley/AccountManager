namespace AccountManagerCore
{
    public class TypicalTransaction
    {
        private readonly List<Transaction> transactions = [];

        public string? Description { get; set; }


        public TypicalTransactionRepeatType RepeatType { get; }

        private TypicalTransaction(TypicalTransactionRepeatType repeatType, Transaction tran1, Transaction tran2)
        {
            RepeatType = repeatType;
            transactions.Add(tran1);
            transactions.Add(tran2);
        }

        public static IEnumerable<TypicalTransaction> FindTypicalTransactions(IEnumerable<Transaction> transactions)
        {
            Dictionary<Transaction, TypicalTransaction?> checkedTransactions = [];

            foreach (Transaction transaction in transactions)
            {
                Transaction? similarTransaction = checkedTransactions.Keys.FirstOrDefault(t => MatchesDescription(t, transaction));

                TypicalTransaction? typicalTransaction = null;

                // If a transaction with a matching description is found...
                if (similarTransaction != null)
                {
                    // ...find the ascosiated typical transaction...
                    typicalTransaction = checkedTransactions[similarTransaction];
                    TypicalTransactionRepeatType repeatType = TypicalTransactionRepeatType.None;

                    // ...check if they have a repeating patern...
                    if (similarTransaction.Date.Day == transaction.Date.Day)
                        repeatType |= TypicalTransactionRepeatType.Day;
                    if (similarTransaction.Date.DayOfWeek == transaction.Date.DayOfWeek)
                        repeatType |= TypicalTransactionRepeatType.Weekday;

                    // ...if there isn't an ascosiated typical transaction create one... 
                    if (typicalTransaction == null)
                        typicalTransaction = new(repeatType, similarTransaction, transaction);
                    // ...if the transaction exists and has the same repeating pattern add the transaction
                    else if (repeatType.HasFlag(typicalTransaction.RepeatType))
                        typicalTransaction.AddTransaction(transaction);
                }

                checkedTransactions.Add(transaction, typicalTransaction);
            }

            return checkedTransactions.Values.Where(t => t != null).Distinct()!;
        }

        private static bool MatchesDescription(Transaction transaction1, Transaction transaction2)
        {
            if (transaction1.Description == transaction2.Description)
            {
                return true;
            }

            // TODO: Add partial description matches

            return false;
        }


        public IEnumerable<Transaction> GetTransactions() => new List<Transaction>(transactions);

        public bool AddTransaction(Transaction transaction)
        {
            if (!transactions.Contains(transaction))
            {
                transactions.Add(transaction);
                return true;
            }
            return false;
        }

        public bool RemoveTransaction(Transaction transaction) => transactions.Remove(transaction);



        public TypicalTransactionValueType ValueType { get; set; } = TypicalTransactionValueType.Median;

        public decimal GetValue() => ValueType switch
        {
            TypicalTransactionValueType.Median => GetMedian(),
            TypicalTransactionValueType.Mode => GetMode(),
            TypicalTransactionValueType.Max => GetMax(),
            TypicalTransactionValueType.Min => GetMin(),
            _ => throw new NotImplementedException()
        };

        public decimal GetMedian()
        {
            Dictionary<decimal, int> counts = [];

            foreach (Transaction transaction in transactions)
            {
                counts.TryAdd(transaction.Amount, 0);

                counts[transaction.Amount]++;
            }


            // If all the values are different take the mode
            if (counts.Values.All(x => x == 1))
                return GetMode();


            decimal median = 0;
            int largestCount = 0;

            foreach (decimal amount in counts.Keys)
            {
                if (counts[amount] > largestCount)
                {
                    median = amount;
                    largestCount = counts[amount];
                }
            }

            return median;
        }
        public decimal GetMode() => transactions.Average(t => t.Amount);
        public decimal GetMax() => transactions.Max(t => t.Amount);
        public decimal GetMin() => transactions.Min(t => t.Amount);
    }

    [Flags]
    public enum TypicalTransactionRepeatType
    {
        None = 0, // No repeating patern can be found
        Day = 1, // Repeats on the same day of a month
        Weekday = 2, // Repeats on the same weekday
    }

    public enum TypicalTransactionValueType
    {
        Median,
        Mode,
        Max,
        Min
    }
}
