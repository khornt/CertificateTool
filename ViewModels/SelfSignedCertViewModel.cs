using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CertTool.Backend.OAuthTest;
using CertTool.Commands;
using CertTool.Models.Dto;
using CertTool.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace CertTool.ViewModels
{
    public class SelfSignedCertViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand TokenCommand { get; private set; }
        public ICommand SelfSignedCertCommand { get; private set; }
        private readonly INavigationService _navigationService;
        private readonly TokenService _tokenService;
        
        private string _hostname;
        private string _rootPwd;
        private string _certPwd;
        


        public SelfSignedCertViewModel(INavigationService navigationService, TokenService tokenService)
        {
            _navigationService = navigationService;
            _tokenService = tokenService;
            TokenCommand = new TaskDelegateCommand(ExecuteTokenCommand, CanExecute);
            SelfSignedCertCommand = new TaskDelegateCommand(ExecuteCreateSertificateCommand, CanExecute);
        }

        private async Task ExecuteTokenCommand()
        {
            

            var response = await _tokenService.GetToken();

            string formattedJson = JToken.Parse(response).ToString(Formatting.Indented);

            Clipboard.SetText(formattedJson);
        }
        
        private async Task ExecuteCreateSertificateCommand()
        {
            var pfxFilePath = "c:\\projects\\cert";
            
            CertificateGenerator.Generate(pfxFilePath, _hostname, _rootPwd, _certPwd);
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool CanExecute()
        {
            return true;
        }
        
        public string RootPwd
        {
            get { return _rootPwd; }
            set { _rootPwd = value; OnPropertyChanged("RootPwd"); }
        }
        
        public string CertPwd
        {
            get { return _certPwd; }
            set { _certPwd = value; OnPropertyChanged("CertPwd"); }
        }

        public string Hostname
        {
            get { return _hostname; }
            set { _hostname = value; OnPropertyChanged("Hostname"); }
        }
    }
}
