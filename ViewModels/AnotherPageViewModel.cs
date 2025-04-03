using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PemConverter.Commands;
using PemConverter.Services;


namespace PemConverter.ViewModels
{
    public class AnotherPageViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand PemCommand { get; private set; }
        private readonly INavigationService _navigationService;
        

        public AnotherPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PemCommand = new CommandBase(ExecutePemCommand);
        }

        private void ExecutePemCommand(object obj)
        {
            MessageBox.Show("jadda!");
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
