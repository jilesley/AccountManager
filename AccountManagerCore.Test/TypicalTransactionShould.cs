namespace AccountManagerCore.Test
{
    public class TypicalTransactionShould
    {
        private Account SampleAcount { get; set; }
        private static string SampleTransactionDescription => "Sample Transaction";


        [SetUp]
        public void Setup()
        {
            AccountManager.RemoveAccount(SampleAcount);
            SampleAcount = AccountManager.AddAccount();
        }

        private void AssertCorrectTypicalTransactionTypeCreated(DateTime tran1Date, DateTime tran2Date, TypicalTransactionRepeatType repeatType)
        {
            Transaction sampleTran1 = SampleAcount.AddTransaction(SampleTransactionDescription, tran1Date, 0);
            Transaction sampleTran2 = SampleAcount.AddTransaction(SampleTransactionDescription, tran2Date, 0);

            var results = TypicalTransaction.FindTypicalTransactions([sampleTran1, sampleTran2], []);

            var result = results.FirstOrDefault();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsNewTypicalTransaction, Is.True);

            var newTypicalTransaction = AccountManager.AddTypicalTransactions(results).FirstOrDefault();

            Assert.That(newTypicalTransaction, Is.Not.Null);
            Assert.That(newTypicalTransaction.RepeatType, Is.EqualTo(repeatType));
            Assert.That(newTypicalTransaction.GetTransactions(), Does.Contain(sampleTran1));
            Assert.That(newTypicalTransaction.GetTransactions(), Does.Contain(sampleTran2));
        }

        [Test]
        public void BeCreatedWhenTransactionsHaveMatchingDescriptions()
            => AssertCorrectTypicalTransactionTypeCreated(new(2024, 1, 1), new(2024, 2, 3), TypicalTransactionRepeatType.None);

        [Test]
        public void BeCreatedWhenTransactionsHaveMatchingDayOfTheMonth()
            => AssertCorrectTypicalTransactionTypeCreated(new(2024, 1, 15), new(2024, 2, 15), TypicalTransactionRepeatType.Day);

        [Test]
        public void BeCreatedWhenTransactionsHaveMatchingDayOfTheWeek()
        {
            DateTime baseTime = new(2024, 1, 1);
            // Do test with time 5 weeks later, so that its the same day of the week
            AssertCorrectTypicalTransactionTypeCreated(baseTime, baseTime.AddDays(7 * 5), TypicalTransactionRepeatType.Weekday);
        }

        [Test]
        public void AddMatchingToExistingTypicalTransactions()
        {
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 1, 1), 0);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 2, 1), 0);

            var existingTypicalTransactions = AccountManager.AddTypicalTransactions(TypicalTransaction.FindTypicalTransactions(SampleAcount.GetTransactions(), []));

            Transaction newTran = SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 3, 1), 0);

            var results = TypicalTransaction.FindTypicalTransactions([newTran], existingTypicalTransactions);

            var result = results.FirstOrDefault();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsNewTypicalTransaction, Is.False);

            var existingTypicalTransaction = AccountManager.AddTypicalTransactions(results).FirstOrDefault();

            Assert.That(existingTypicalTransaction, Is.Not.Null);
            Assert.That(existingTypicalTransactions, Does.Contain(existingTypicalTransaction));
            Assert.That(existingTypicalTransaction.GetTransactions(), Does.Contain(newTran));
        }

        [Test]
        public void GroupMultipleTransactionsIntoOneTypical()
        {
            for (int i = 1; i <= 12; i++)
            {
                SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, i, 1), 0);
            }

            var results = AccountManager.AddTypicalTransactions(TypicalTransaction.FindTypicalTransactions(SampleAcount.GetTransactions(), []));

            Assert.That(results.Count, Is.EqualTo(1));

            var actual = results.FirstOrDefault();

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.GetTransactions().Count, Is.EqualTo(12));
        }

        [Test]
        public void CreateMultipleTypicalTransactionsWhenAppropriate()
        {
            for (int i = 1; i <= 5; i++)
            {
                SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 1, i), 0);
                SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 2, i), 0);
            }

            var results = AccountManager.AddTypicalTransactions(TypicalTransaction.FindTypicalTransactions(SampleAcount.GetTransactions(), []));

            Assert.That(results.Count, Is.EqualTo(5));

            foreach (var result in results)
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.GetTransactions().Count, Is.EqualTo(2));
            }
        }

        private TypicalTransaction AddAverageTestTransactions()
        {
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 1, 1), -5);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 2, 1), -3);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 3, 1), -3);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 4, 1), 10);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 5, 1), 12);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 6, 1), 14);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 7, 1), 12);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 8, 1), 12);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 9, 1), 100);
            SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, 10, 1), -100);

            var results = AccountManager.AddTypicalTransactions(TypicalTransaction.FindTypicalTransactions(SampleAcount.GetTransactions(), []));

            return results.First();
        }

        [Test]
        public void GetCorrectMedian()
        {
            TypicalTransaction actual = AddAverageTestTransactions();

            Assert.That(actual.GetMedian(), Is.EqualTo(12));
        }

        [Test]
        public void GetCorrectMode()
        {
            TypicalTransaction actual = AddAverageTestTransactions();

            Assert.That(actual.GetMode(), Is.EqualTo(4.9));
        }

        [Test]
        public void GetCorrectMax()
        {
            TypicalTransaction actual = AddAverageTestTransactions();

            Assert.That(actual.GetMax(), Is.EqualTo(100));
        }

        [Test]
        public void GetCorrectMin()
        {
            TypicalTransaction actual = AddAverageTestTransactions();

            Assert.That(actual.GetMin(), Is.EqualTo(-100));
        }

        [Test]
        public void GetModeWhenAllTransactionsHaveDifferentAmountsAndGetMedianIsUsed()
        {
            int total = 0;
            for (int i = 1; i <= 10; i++)
            {
                SampleAcount.AddTransaction(SampleTransactionDescription, new(2024, i, 1), i);
                total += i;
            }

            var results = AccountManager.AddTypicalTransactions(TypicalTransaction.FindTypicalTransactions(SampleAcount.GetTransactions(), []));

            TypicalTransaction actual = results.First();

            double expected = total / 10.0;

            Assert.That(actual.GetMedian(), Is.EqualTo(expected));
        }
    }
}
