using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AccountManager.WPF.ViewModels
{
    public class AddTransactionsViewModel : ObservableRecipient
    {
        private string inputText;


        public ObservableCollection<Transaction> NewTransactions { get; } = new();


        public AddTransactionsViewModel()
        {
            AddToDatabaseCommand = new RelayCommand(AddToDatabase);
        }

        public void InputTextChanged()
        {
            NewTransactions.Clear();

            IEnumerable<Transaction> transactions = DatabaseManager.ParseTransactions(InputText);

            foreach (Transaction transaction in transactions)
            {
                NewTransactions.Add(transaction);
            }
        }


        public string InputText
        {
            get => inputText;
            set
            {
                if (SetProperty(ref inputText, value))
                {
                    InputTextChanged();
                }
            }
        }


        public void AddToDatabase()
        {
            //WeakReferenceMessenger.Default.Send<;
        }

        public ICommand AddToDatabaseCommand { get; }
    }
}
