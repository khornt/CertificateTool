
using PemConverter.Backend.LetsEncrypt.WebClient;
using PemConverter.Models;
using PemConverter.ViewModels;

namespace PemConverter.Commands
{
    //public class CertificateCommand : AsyncCommandBase
    //{

        //private readonly CreateCertificateViewModel _createCertificateViewModel;
        //private readonly AppConfig _config;

        //private readonly AcmeClient _acmeClient;

        //public CertificateCommand(CreateCertificateViewModel createCertificateViewModel, AcmeClient acmeClient, AppConfig config, Action<Exception> onException) : base(onException)
        //{
        //    _createCertificateViewModel = createCertificateViewModel;
        //    _config = config;
        //    _acmeClient = acmeClient;
        //}

        //protected override async Task ExecuteAsync(object parameter)
        //{
        //    _createCertificateViewModel.State = 1;
            
        //    var account = await _acmeClient.CreateNewAccountAsync(_config.ContactEmail);
        //    var order = await _acmeClient.NewOrderAsync(account, fqdn);

        //    var a = "gikk alt ok?";

        //    _createCertificateViewModel.StatusLabel = "OK??";

        //}
    //}
}
