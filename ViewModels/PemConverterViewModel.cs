
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PemConverter.Models;
using PemConverter.Models.Jwk;
using PemConverter.Services;

namespace PemConverter.ViewModels
{
    public class PemConverterViewModel : ViewModelBase
    {
        private string _selectedFilePath = "";
        private string _publicKey = "";
        private string _fileLabel = "";
        private string _kid = "";
        private string _jwk = "";
        private string _rsaKey = "";

        private readonly JwkCreator _jwkCreator;
        private readonly PubKeyHandler _pubKeyHandler;

        private readonly INavigationService _navigationService;

        public ICommand SelectFileCommand { get; private set; }
        public ICommand SelectCertificateCommand { get; private set; }
        public ICommand CreateJwkFileCommand { get; private set; }
        public ICommand NavigateTestPage { get; private set; }

        public PemConverterViewModel(
            JwkCreator jwkCreator,
            PubKeyHandler pubKeyHandler,
            INavigationService navigationService,
            AppConfig config)
        {
            SelectFileCommand = new DelegateCommand(ExecuteSelectFileCommand);
            CreateJwkFileCommand = new DelegateCommand(ExecuteCreateJwkCommand);
            SelectCertificateCommand = new DelegateCommand(ExecuteSelectCertificateCommand);
            
            _jwkCreator = jwkCreator;
            _pubKeyHandler = pubKeyHandler;
            _navigationService = navigationService;


            AlgorithmDropDown = config.RsaKeys;
            NavigateTestPage = new DelegateCommand(ExecuteTestCommand); ;
        }

  

        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set { _selectedFilePath = value; OnPropertyChanged("SelectedFilePath"); }
        }

        public string FileLabel
        {
            get { return _fileLabel; }
            set { _fileLabel = value; OnPropertyChanged("FileLabel"); }
        }


        public string Algorithm
        {
            get { return _rsaKey; }
            set { _rsaKey = value; OnPropertyChanged("Algorithm"); }
        }

        public string PublicKey
        {
            get { return _publicKey; }
            set { _publicKey = value; OnPropertyChanged("PublicKey"); }
        }


        public string Kid
        {
            get { return _kid; }
            set { _kid = value; OnPropertyChanged("Kid"); }
        }

        public string Jwk
        {
            get { return _jwk; }
            set { _jwk = value; OnPropertyChanged("Jwk"); }
        }

        public List<string> AlgorithmDropDown { get; }

        

        
        private void ExecuteSelectCertificateCommand()
        {
            
            if (!PreValidateInput()) return;

            var key = _pubKeyHandler.GetPublicKeyFromSertificate(Kid);

            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("Could not find Certificate: " + Kid, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
             

            var response = _jwkCreator.CreateJwk(key, Kid, Algorithm);

            if (response.Success == true && response.DigDirJwkString != null)
            {
                Jwk = response.DigDirJwkString;
            }
            else
            {

                MessageBox.Show(response.ErrorCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return;
        }
       
        private void ExecuteCreateJwkCommand()
        {
           

            Kid = Kid.ToUpper();
            //var response = _jwkCreator.CreateJwk(_publicKey, Kid, Algorithm);

            if (!string.IsNullOrEmpty(Jwk))
            {                 
                if (_jwkCreator.SaveToFile(Jwk))
                {
                    MessageBox.Show("Result saved successfully!", "Success");
                }
            }
            else
            {
                MessageBox.Show("Failed to save Jwk to file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return;
        }

        private bool PreValidateInput()
        {
            if (string.IsNullOrEmpty(Kid))
            {
            
                MessageBox.Show("Enter Certificate Kid!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            if (string.IsNullOrEmpty(Algorithm))
            {
                MessageBox.Show("Select RSA Key!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            return true;
        }

        private void ExecuteSelectFileCommand()
        {
            if (!PreValidateInput()) return;

            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                FileLabel = "Selected File: " + openFileDialog.SafeFileName;
                PublicKey = _pubKeyHandler.GetPublicKey(_selectedFilePath);
            } else
            {
                return;
            }

            

            var response = _jwkCreator.CreateJwk(_publicKey, Kid, Algorithm);

            if (response.Success == true && response.DigDirJwkString != null)
            {
                Jwk = response.DigDirJwkString;
            }
            else
            {
                MessageBox.Show(response.ErrorCode, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return;

        }

        private void ExecuteTestCommand()
        {
            _navigationService.Navigate();

        }
        
    }
}
