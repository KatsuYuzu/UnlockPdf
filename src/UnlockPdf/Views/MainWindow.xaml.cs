using System.ComponentModel;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using UnlockPdf.ViewModels;

namespace UnlockPdf.Views;

public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }

    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(MainWindowViewModel.Password) &&
            DataContext is MainWindowViewModel viewModel &&
            PasswordBox.Password != viewModel.Password)
        {
            PasswordBox.Password = viewModel.Password;
        }
    }
}
