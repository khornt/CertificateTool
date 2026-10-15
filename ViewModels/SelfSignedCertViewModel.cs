using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CertTool.Backend.OAuthTest;
using CertTool.Commands;
using CertTool.Models.Dto;
using CertTool.Services;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
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
        
        public Array Algorithms { get; } = Enum.GetValues(typeof(CertificateGenerator.KeyAlgorithm));
        public string[] SignatureHashes { get; } = ["SHA256", "SHA384", "SHA512"];
        public ObservableCollection<int> KeySizes { get; } = [2048, 3072, 4096];

        private CertificateGenerator.KeyAlgorithm _selectedAlgorithm = CertificateGenerator.KeyAlgorithm.Rsa;
        public CertificateGenerator.KeyAlgorithm SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set
            {
                if (_selectedAlgorithm == value) return;
                _selectedAlgorithm = value;
                OnPropertyChanged();
                KeySizes.Clear();
                foreach (var size in CertificateGenerator.AllowedKeySizes(value))
                    KeySizes.Add(size);
                SelectedKeySize = KeySizes[0];
                SelectedSignatureHash = "SHA256";
            }
        }

        private int _selectedKeySize = 2048;
        public int SelectedKeySize
        {
            get => _selectedKeySize;
            set { _selectedKeySize = value; OnPropertyChanged(); }
        }

        private string _selectedSignatureHash = "SHA256";
        public string SelectedSignatureHash
        {
            get => _selectedSignatureHash;
            set { _selectedSignatureHash = value; OnPropertyChanged(); }
        }

        private string _commonName;
        private string _orgUnit;
        private string _org;
        private string _localilty;
        private string _state;
        private string _country;
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
            const string pfxFilePath = @"C:\projects\cert";
            var hostname = CommonName;
            var subject = $"CN={CommonName}, OU={OrgUnit}, O={Org}, " +
                          $"L={Localilty}, S={State}, C={Country}";
            var rootPassword = RootPwd;
            var certPassword = CertPwd;
            var algorithm = SelectedAlgorithm;
            var keySize = SelectedKeySize;
            var hash = SelectedSignatureHash;

            try
            {
                await Task.Run(() => CertificateGenerator.Generate(
                    pfxFilePath, hostname, subject, rootPassword, certPassword,
                    algorithm: algorithm, keySize: keySize, signatureHash: hash));
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Certificate Generation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Certificate Generation Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        public string CommonName
        {
            get { return _commonName; }
            set { _commonName = value; OnPropertyChanged("CommonName"); }
        }
        
        public string OrgUnit
        {
            get { return _orgUnit; }
            set { _orgUnit = value; OnPropertyChanged("OrgUnit"); }
        }
        public string Org
        {
            get { return _org; }
            set { _org = value; OnPropertyChanged("Org"); }
        }
        public string Localilty
        {
            get { return _localilty; }
            set { _localilty = value; OnPropertyChanged("Localilty"); }
        }
        public string State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged("State"); }
        }
        public string Country
        {
            get { return _country; }
            set { _country = value; OnPropertyChanged("Country"); }
        }
    }
}
