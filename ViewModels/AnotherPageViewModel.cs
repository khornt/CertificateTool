using PemConverter.Backend.OAuthTest;
using PemConverter.Commands;
using PemConverter.Models.Dto;
using PemConverter.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace PemConverter.ViewModels
{
    public class AnotherPageViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand TokenCommand { get; private set; }
        private readonly INavigationService _navigationService;
        private readonly TokenService _tokenService;


        public AnotherPageViewModel(INavigationService navigationService, TokenService tokenService)
        {
            _navigationService = navigationService;
            _tokenService = tokenService;
            TokenCommand = new TaskDelegateCommand(ExecuteTokenCommand, CanExecute);
        }

        private async Task ExecuteTokenCommand()
        {
            

            var response = await _tokenService.GetToken();

            var test = "";

            //var token = await _acmeService.CreateChallengeAsync(FQDN, Email);

        }


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool CanExecute()
        {
            return true;
        }


    }
}
