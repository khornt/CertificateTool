using System.Text;
using System.Windows.Input;
using PemConverter.Backend.LetsEncrypt.IO;
using PemConverter.Commands;
using PemConverter.Models;
using PemConverter.Models.Dto;
using PemConverter.Services;
using PemConverter.Services.Acme;


namespace PemConverter.ViewModels
{
    public class CreateCertificateViewModel : ViewModelBase
    {

        private readonly INavigationService _navigationService;
        private readonly IAcmeService _acmeService;

        private readonly AppConfig _config;
        private ChallengeStore _challengeStore;
        private string _fqdn;
        private string _email;
        private string _password;
        private int _state;
        private string _challengeText;        
        
        public ICommand ChallengeCommand { get; private set; }
        public ICommand CreateCertificateCommand { get; private set; }
        public ICommand CreateChallengeCommand { get; private set; }


        //DI!!!!
        private LocalStorage _lfh = new LocalStorage();


        public CreateCertificateViewModel(INavigationService navigationService, IAcmeService acmeService,
            AppConfig config)
        {
            _navigationService = navigationService;
            _acmeService = acmeService;
            _config = config;

            CreateChallengeCommand = new TaskDelegateCommand(ExecuteCreateChallengeCommand, CanExecuteCreateChallenge).ObservesProperty(() => State); 
            ChallengeCommand = new TaskDelegateCommand(ExecuteChallengeCommand, CanExecuteCallenge).ObservesProperty(() => State);
            CreateCertificateCommand = new TaskDelegateCommand(ExecuteCreateCertificate, CanExecuteCertificate).ObservesProperty(() => State);

            _state = 0;
        }

        private bool CanExecuteCreateChallenge()
        {
            if (_state == 0) return true;

            return false;
        }

        private bool CanExecuteCallenge()
        {
            if (_state == 1) return true;
            
            return false;

        }

        private bool CanExecuteCertificate()
        {
            if (_state == 2) return true;

            return false;
        }
                

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged("Email"); }
        }


        public string FQDN
        {
            get { return _fqdn; }
            set { _fqdn = value; OnPropertyChanged("FQDN"); }
        }

        public int State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged("State"); }
        }
        

        public string ChallengeText
        {
            get { return _challengeText; }
            set { _challengeText = value; OnPropertyChanged("ChallengeText"); }
        }


        public ChallengeStore ChallengeStore
        {
            get { return _challengeStore; }
            set
            {
                _challengeStore = value; OnPropertyChanged(nameof(ChallengeStore));
            }
        }

        private async Task ExecuteCreateChallengeCommand()
        {

            try
            {
                ChallengeText = null;

                ChallengeStore = await _acmeService.CreateChallengeAsync(FQDN, Email);
                
                
                var sb = new StringBuilder(256);
                foreach (var item in ChallengeStore.Challenges)
                {
                    sb.AppendLine(string.Format("Key: {0}", item.DnsKey));
                    sb.AppendLine(string.Format("Value: {0}", item.VerificationValue));
                    sb.AppendLine();
                }                
                ChallengeText = sb.ToString();
                State = 1;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ChallengeText)) ChallengeText += "\n";

                ChallengeText   += ex.Message;
            }
        }

        private async Task ExecuteChallengeCommand()
        {
            try
            {
                var status = await _acmeService.ValidateChallengeAsync(ChallengeStore);
                if (status)
                {
                    if (!string.IsNullOrEmpty(ChallengeText)) ChallengeText += "\n";

                    ChallengeText += "Verified!";
                    State = 2;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ChallengeText)) ChallengeText += "\n";
                ChallengeText += ex.Message;
                State = 0;
            }
        }

        private async Task ExecuteCreateCertificate()
        {

            var order = ChallengeStore.Order;
            var account = ChallengeStore.Account;
            
            try
            {

                var certificate = await _acmeService.GenerateCertificateAsync(account, order, FQDN);

                // Save files locally
                await _lfh.WriteAsync(FQDN + ".pfx", certificate.GeneratePfx(_config.CertificatePassword));
                await _lfh.WriteAsync(FQDN + ".crt", certificate.GenerateCrt(_config.CertificatePassword));
                await _lfh.WriteAsync(FQDN + ".crt.pem", certificate.GenerateCrtPem(_config.CertificatePassword));
                await _lfh.WriteAsync(FQDN + ".key.pem", certificate.GenerateKeyPem());

                if (!string.IsNullOrEmpty(ChallengeText)) ChallengeText += "\n";

                ChallengeText += "Certificate Generated!!";

            } catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ChallengeText)) ChallengeText += "\n";
                ChallengeText += ex.Message;                
            }
            State = 0;

        }
    }
}
