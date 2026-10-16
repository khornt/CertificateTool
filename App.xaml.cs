using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using CertTool.Models;
using CertTool.Models.Jwk;
using CertTool.Services;
using CertTool.Stores;
using CertTool.ViewModels;
using CertTool.Backend.LetsEncrypt.IO;
using CertTool.Backend.LetsEncrypt.Entities;
using CertTool.Backend.LetsEncrypt.WebClient;
using CertTool.Services.Acme;
using CertTool.Backend.OAuthTest;

namespace CertTool;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();

        services.AddSingleton<NavigationStore>();
        services.AddSingleton<JwkCreator>();
        services.AddSingleton<PubKeyHandler>();
        services.AddSingleton<AppConfig>();

        // Register each destination as its own typed service, instead of
        // three competing registrations of INavigationService.
        services.AddSingleton<NavigationService<PemConverterViewModel>>(sp =>
            new NavigationService<PemConverterViewModel>(
                sp.GetRequiredService<NavigationStore>(),
                () => sp.GetRequiredService<PemConverterViewModel>()));

        services.AddSingleton<NavigationService<CreateCertificateViewModel>>(sp =>
            new NavigationService<CreateCertificateViewModel>(
                sp.GetRequiredService<NavigationStore>(),
                () => sp.GetRequiredService<CreateCertificateViewModel>()));

        services.AddSingleton<NavigationService<SelfSignedCertViewModel>>(sp =>
            new NavigationService<SelfSignedCertViewModel>(
                sp.GetRequiredService<NavigationStore>(),
                () => sp.GetRequiredService<SelfSignedCertViewModel>()));

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>(sp => new MainWindow
        {
            DataContext = sp.GetRequiredService<MainViewModel>()
        });

        services.AddTransient<CreateCertificateViewModel>();
        services.AddTransient<SelfSignedCertViewModel>();
        services.AddTransient<PemConverterViewModel>();

        services.AddSingleton<IAcmeService, AcmeService>();
        services.AddSingleton<TokenService>();
        services.AddSingleton<AcmeClient>(new AcmeClient(ApiEnvironment.LetsEncryptV2));
        services.AddSingleton<LocalStorage>();

        _serviceProvider = services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _serviceProvider.GetRequiredService<NavigationService<PemConverterViewModel>>().Navigate();
        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}
