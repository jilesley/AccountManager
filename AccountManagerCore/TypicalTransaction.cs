namespace AccountManagerCore
{
    public class TypicalTransaction
    {
        private readonly List<Transaction> transactions = [];

        public string? Description { get; set; }


        public TypicalTransactionRepeatType RepeatType { get; }

        private TypicalTransaction(TypicalTransactionRepeatType repeatType)
        {
            RepeatType = repeatType;
        }


        public class FindTypicalTransactionResult
        {
            internal FindTypicalTransactionResult(bool isNewTypicalTransaction, TypicalTransaction typicalTransaction)
            {
                IsNewTypicalTransaction = isNewTypicalTransaction;
                TypicalTransaction = typicalTransaction;
            }
            internal FindTypicalTransactionResult(bool isNewTypicalTransaction, TypicalTransaction typicalTransaction, Transaction transaction)
            {
                IsNewTypicalTransaction = isNewTypicalTransaction;
                TypicalTransaction = typicalTransaction;
                newTransactions.Add(transaction);
            }

            private readonly List<Transaction> newTransactions = [];
            private bool newTransactionsAdded;
            public bool NewTransactionsAdded { get => newTransactionsAdded; }
            public bool IsNewTypicalTransaction { get; }
            public TypicalTransaction TypicalTransaction { get; }

            internal void AddNewTransaction(Transaction transaction) => newTransactions.Add(transaction);

            public IEnumerable<Transaction> GetNewTransactions() => new List<Transaction>(newTransactions);

            public bool Matches(Transaction transaction) => TypicalTransaction.Matches(transaction) ||
                newTransactions.Any(t => MatchesDescription(t, transaction) && MatchesTiming(TypicalTransaction.RepeatType, t, transaction));

            public bool AddNewTransactionsToTypical()
            {
                if (!newTransactionsAdded)
                {
                    TypicalTransaction.transactions.AddRange(newTransactions);
                    newTransactionsAdded = true;
                    return true;
                }
                return false;
            }
        }


        public static IEnumerable<FindTypicalTransactionResult> FindTypicalTransactions(IEnumerable<Transaction> transactions, IEnumerable<TypicalTransaction> existingTypicalTransactions)
        {
            List<FindTypicalTransactionResult> results = [];
            List<Transaction> checkedNonTypicalTransactions = [];

            // Add all existing typical transactions to the results
            foreach (TypicalTransaction typTran in existingTypicalTransactions)
                results.Add(new(false, typTran));

            foreach (Transaction transaction in transactions)
            {
                // If the transaction doesn't have a typical transaction...
                if (transaction.TypicalTransaction == null)
                {
                    // ...see if it matches any existing ones in the results...
                    FindTypicalTransactionResult? matchingResult = results.FirstOrDefault(tt => tt.Matches(transaction));

                    if (matchingResult != null)
                    {
                        // ...if it does add it as a new transaction...
                        matchingResult.AddNewTransaction(transaction);
                    }
                    else
                    {
                        // ...else check if any of the checked non typical transaction have a matching description...
                        IEnumerable<Transaction> similarTransactions = checkedNonTypicalTransactions.Where(t => MatchesDescription(t, transaction));

                        foreach (Transaction simTran in similarTransactions)
                        {
                            // ...for any that match; check if they have a matching repeat type...
                            TypicalTransactionRepeatType? repeatType = MatchingRepeatType(simTran, transaction);

                            if (repeatType != null)
                            {
                                // ...if they do; create a new typical transaction add it to the results and add the transactions...
                                matchingResult = new(true, new(repeatType.Value));
                                matchingResult.AddNewTransaction(transaction);
                                matchingResult.AddNewTransaction(simTran);

                                results.Add(matchingResult);

                                // ...remove the similar transactions from the checked non typical transactions...
                                checkedNonTypicalTransactions.Remove(simTran);

                                // ...and stop cycling through the similar transactions
                                break;
                            }
                        }
                    }

                    if (matchingResult == null)
                        checkedNonTypicalTransactions.Add(transaction);
                }
            }

            return results;
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

        private static bool MatchesTiming(TypicalTransactionRepeatType repeatType, Transaction transaction1, Transaction transaction2)
        {
            switch (repeatType)
            {
                case TypicalTransactionRepeatType.None:
                    return true;

                case TypicalTransactionRepeatType.Day:
                    if (transaction1.Date.Day == transaction2.Date.Day)
                        return true;
                    break;

                case TypicalTransactionRepeatType.Weekday:
                    if (transaction1.Date.DayOfWeek == transaction2.Date.DayOfWeek)
                        return true;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return false;
        }

        private static TypicalTransactionRepeatType? MatchingRepeatType(Transaction transaction1, Transaction transaction2)
        {
            foreach (TypicalTransactionRepeatType repeatType in Enum.GetValues(typeof(TypicalTransactionRepeatType)))
            {
                if (repeatType != TypicalTransactionRepeatType.None && MatchesTiming(repeatType, transaction1, transaction2))
                    return repeatType;
            }

            if (MatchesTiming(TypicalTransactionRepeatType.None, transaction1, transaction1))
                return TypicalTransactionRepeatType.None;

            return null;
        }

        private bool Matches(Transaction transaction)
            => transactions.Any(t => MatchesDescription(t, transaction)) && MatchesTiming(RepeatType, transactions.First(), transaction);


        public IEnumerable<Transaction> GetTransactions() => new List<Transaction>(transactions);

        public bool AddTransaction(Transaction transaction)
        {
            if (!transactions.Contains(transaction) && transaction.TypicalTransaction == null)
            {
                transactions.Add(transaction);
                transaction.TypicalTransaction = this;
                return true;
            }
            return false;
        }

        public void AddTransaction(IEnumerable<Transaction> transactions)
        {
            foreach (Transaction transaction in transactions)
            {
                AddTransaction(transaction);
            }
        }

        public bool RemoveTransaction(Transaction transaction)
        {
            if (transactions.Remove(transaction))
            {
                transaction.TypicalTransaction = null;
                return true;
            }
            return false;
        }



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
        None = 1, // No repeating patern can be found
        Day = 2, // Repeats on the same day of a month
        Weekday = 4, // Repeats on the same weekday
    }

    public enum TypicalTransactionValueType
    {
        Median,
        Mode,
        Max,
        Min
    }
}
