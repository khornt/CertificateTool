using System.Windows.Input;
using PemConverter.Commands;
using PemConverter.Services;
using PemConverter.Stores;

namespace PemConverter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly INavigationService _pemNavigationService;
        private readonly INavigationService _testPageNavigationService;
        private readonly INavigationService _anotherPageNavigationService;
        


        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public ICommand PemConverterCommand { get; internal set; }
        public ICommand TestPageCommand { get; internal set; }
        public ICommand AnotherPageCommand { get; internal set; }



        public MainViewModel(NavigationStore navigationStore, 
            INavigationService pemNavigationService, 
            INavigationService testPageNavigationService,
            INavigationService anotherPageNavigationService
            )
        {
            _navigationStore = navigationStore;
            _pemNavigationService = pemNavigationService;
            _testPageNavigationService = testPageNavigationService;
            _anotherPageNavigationService = anotherPageNavigationService;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            PemConverterCommand = new CommandBase(ExecutePemConverterCommand);
            TestPageCommand = new CommandBase(ExecuteTestPageCommand);
            AnotherPageCommand = new CommandBase(ExecuteAnotherPageCommand);
        }

        private void ExecuteTestPageCommand(object obj)
        {
            
            _testPageNavigationService.Navigate();
        }

        private void ExecuteAnotherPageCommand(object obj)
        {
            _anotherPageNavigationService.Navigate();
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
