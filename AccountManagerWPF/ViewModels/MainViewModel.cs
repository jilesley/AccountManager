using AccountManager.WPF.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountManager.WPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private Account selectedAccount;
        public Account SelectedAccount
        {
            get => selectedAccount;
            set 
            {
                if (SetProperty(ref selectedAccount, value))
                {
                    UpdateTransactions();
                }
            }
        }


        public ObservableCollection<Account> Accounts { get; } = new();

        public ObservableCollection<Transaction> Transactions { get; } = new();


        public MainViewModel()
        {
            AddTransactionCommand = new RelayCommand(AddTransaction);

            UpdateAccounts(@"C:\Users\James\Downloads\TempDatabase.xml");
        }

        private void UpdateAccounts(string databasePath)
        {
            Accounts.Clear();

            IEnumerable<Account> newAccounts = DatabaseManager.ParseDatabase(databasePath);

            foreach (Account account in newAccounts)
            {
                Accounts.Add(account);
            }

            SelectedAccount = Accounts.FirstOrDefault();
        }

        public void UpdateTransactions()
        {
            Transactions.Clear();

            if (SelectedAccount != null)
            {
                foreach (Transaction transaction in SelectedAccount.Transactions)
                {
                    Transactions.Add(transaction);
                }
            }
        }


        public ICommand AddTransactionCommand { get; }


        public void AddTransaction()
        {
            new AddTransactionsView().ShowDialog();
        }
    }
}
