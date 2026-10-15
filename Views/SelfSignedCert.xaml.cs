using System.Windows;
using System.Windows.Controls;
using CertTool.ViewModels;

namespace CertTool.Views;

public partial class SelfSignedCert : UserControl
{
    private bool _updatingControls;

    public SelfSignedCert()
    {
        InitializeComponent();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not SelfSignedCertViewModel vm) return;

        _updatingControls = true;
        try
        {
            RootPasswordBox.Password = vm.RootPwd ?? string.Empty;
            CertPasswordBox.Password = vm.CertPwd ?? string.Empty;
            RootVisibleText.Clear();
            CertVisibleText.Clear();
            RootShowToggle.IsChecked = false;
            CertShowToggle.IsChecked = false;
        }
        finally { _updatingControls = false; }
    }

    private void RootPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (!_updatingControls && DataContext is SelfSignedCertViewModel vm)
            vm.RootPwd = RootPasswordBox.Password;
    }

    private void CertPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (!_updatingControls && DataContext is SelfSignedCertViewModel vm)
            vm.CertPwd = CertPasswordBox.Password;
    }

    private void RootVisibleTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_updatingControls && DataContext is SelfSignedCertViewModel vm)
            vm.RootPwd = RootVisibleText.Text;
    }

    private void CertVisibleTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_updatingControls && DataContext is SelfSignedCertViewModel vm)
            vm.CertPwd = CertVisibleText.Text;
    }

    private void RootToggleChanged(object sender, RoutedEventArgs e) =>
        UpdateVisibility(RootPasswordBox, RootVisibleText, RootShowToggle.IsChecked == true);

    private void CertToggleChanged(object sender, RoutedEventArgs e) =>
        UpdateVisibility(CertPasswordBox, CertVisibleText, CertShowToggle.IsChecked == true);

    private void UpdateVisibility(PasswordBox masked, TextBox visible, bool show)
    {
        if (_updatingControls) return;
        _updatingControls = true;
        try
        {
            if (show)
            {
                visible.Text = masked.Password;
                masked.Visibility = Visibility.Collapsed;
                visible.Visibility = Visibility.Visible;
                visible.Focus();
                visible.CaretIndex = visible.Text.Length;
            }
            else
            {
                masked.Password = visible.Text;
                visible.Visibility = Visibility.Collapsed;
                masked.Visibility = Visibility.Visible;
                visible.Clear();
                masked.Focus();
            }
        }
        finally { _updatingControls = false; }
    }
}
