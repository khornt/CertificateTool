using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using PemConverter.Models;
using PemConverter.Models.Jwk;
using PemConverter.Services;
using PemConverter.Stores;
using PemConverter.ViewModels;
using PemConverter.Backend.LetsEncrypt.IO;
using PemConverter.Backend.LetsEncrypt.Entities;
using PemConverter.Backend.LetsEncrypt.WebClient;
using PemConverter.Services.Acme;

namespace PemConverter
{
    public partial class App : Application
    {
        // https://stackoverflow.com/questions/1922204/open-directory-dialog
        // Todo: Output folder

        private readonly IServiceProvider _serviceProvider;
        private INavigationService _initPage;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<JwkCreator>();
            services.AddSingleton<PubKeyHandler>();
            services.AddSingleton<AppConfig>();

            services.AddSingleton<MainViewModel>(s => new MainViewModel(
                    s.GetRequiredService<NavigationStore>(),
                    PemNavigationService(s),
                    CreateCreateCertificatePageService(s),
                    CreateAnotherPageService(s)
                ));

            services.AddSingleton<MainWindow>(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<INavigationService>(s => PemNavigationService(s));
            services.AddSingleton<INavigationService>(s => CreateCreateCertificatePageService(s));
            services.AddSingleton<INavigationService>(s => CreateAnotherPageService(s));
            
            services.AddTransient<CreateCertificateViewModel>(s => new CreateCertificateViewModel(CreateCreateCertificatePageService(s),
                s.GetRequiredService<AcmeService>(), s.GetRequiredService<AppConfig>()));

            services.AddTransient<AnotherPageViewModel>(s => new AnotherPageViewModel(CreateAnotherPageService(s)));

            services.AddTransient<PemConverterViewModel>(s => 
                new PemConverterViewModel(
                    s.GetRequiredService<JwkCreator>(), 
                  s.GetRequiredService<PubKeyHandler>(),
                    CreateCreateCertificatePageService(s),
                        s.GetRequiredService<AppConfig>()
                ));


            services.AddSingleton<AcmeService>();

            services.AddSingleton<AcmeClient>(new AcmeClient(ApiEnvironment.LetsEncryptV2));
            services.AddSingleton<LocalStorage>();
            




            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            StartUpNavigate(_serviceProvider).Navigate();
            
            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            
            MainWindow.Show();

            base.OnStartup(e);
        }


        private IStartUpNavigate StartUpNavigate(IServiceProvider serviceProvider)
        {
            return new NavigationService<PemConverterViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<PemConverterViewModel>());
        }

        private INavigationService PemNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<PemConverterViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<PemConverterViewModel>());
        }
        
        private INavigationService CreateCreateCertificatePageService(IServiceProvider serviceProvider)
        {
            return new NavigationService<CreateCertificateViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<CreateCertificateViewModel>());
        }

        private INavigationService CreateAnotherPageService(IServiceProvider serviceProvider)
        {
            return new NavigationService<AnotherPageViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<AnotherPageViewModel>());
        }
    }
}
