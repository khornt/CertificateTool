using System.Windows.Input;
using CertTool.Commands;
using CertTool.Services;
using CertTool.Stores;

namespace CertTool.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly INavigationService _pemNavigationService;
        private readonly INavigationService _testPageNavigationService;
        private readonly INavigationService _selfSignedCertPageNavigationService;
        


        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public ICommand PemConverterCommand { get; internal set; }
        public ICommand TestPageCommand { get; internal set; }
        public ICommand SelfSignedCertPageCommand { get; internal set; }



        public MainViewModel(NavigationStore navigationStore, 
            INavigationService pemNavigationService, 
            INavigationService testPageNavigationService,
            INavigationService selfSignedCertPageNavigationService
            )
        {
            _navigationStore = navigationStore;
            _pemNavigationService = pemNavigationService;
            _testPageNavigationService = testPageNavigationService;
            _selfSignedCertPageNavigationService = selfSignedCertPageNavigationService;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            PemConverterCommand = new CommandBase(ExecutePemConverterCommand);
            TestPageCommand = new CommandBase(ExecuteTestPageCommand);
            SelfSignedCertPageCommand = new CommandBase(ExecuteSelfSignedCertPageCommand);
        }

        private void ExecuteTestPageCommand(object obj)
        {
            
            _testPageNavigationService.Navigate();
        }

        private void ExecuteSelfSignedCertPageCommand(object obj)
        {
            _selfSignedCertPageNavigationService.Navigate();
        }

        private void ExecutePemConverterCommand(object obj)
        {
            _pemNavigationService.Navigate();
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
