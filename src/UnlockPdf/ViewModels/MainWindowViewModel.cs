using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.IO;

namespace UnlockPdf.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUnlock))]
    [NotifyPropertyChangedFor(nameof(Status))]
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    private string pdfPath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUnlock))]
    [NotifyPropertyChangedFor(nameof(Status))]
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    private string password = string.Empty;

    [ObservableProperty]
    private string? operationMessage;

    public bool CanUnlock =>
        File.Exists(NormalizePath(PdfPath)) &&
        !string.IsNullOrWhiteSpace(Password);

    public string Status =>
        !string.IsNullOrEmpty(OperationMessage)
            ? OperationMessage
            : !File.Exists(NormalizePath(PdfPath))
                ? "PDF ファイルを選択してください"
                : string.IsNullOrWhiteSpace(Password)
                    ? "パスワードを入力してください"
                    : "[Enter] キーまたは [UNLOCK] ボタンを押してください";

    [RelayCommand]
    private void SelectFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "PDF (*.pdf)|*.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            PdfPath = dialog.FileName;
        }
    }

    [RelayCommand(CanExecute = nameof(CanUnlock))]
    private void Unlock()
    {
        var sourcePath = NormalizePath(PdfPath);
        var directoryPath = Path.GetDirectoryName(sourcePath)!;
        var destinationPath = PathHelper.CreateUniquePath(
            directoryPath,
            Path.GetFileNameWithoutExtension(sourcePath) + "_unlocked.pdf");

        try
        {
            PdfUnlocker.Unlock(sourcePath, destinationPath, Password);
            Password = string.Empty;
            OperationMessage = "PDF ファイルのパスワードを解除しました";
            OnPropertyChanged(nameof(Status));
            PathHelper.OpenExplorerWithSelected(destinationPath);
        }
        catch (PdfPasswordException)
        {
            Password = string.Empty;
            OperationMessage = "正しいパスワードを入力してください";
            OnPropertyChanged(nameof(Status));
        }
        catch (InvalidDataException)
        {
            OperationMessage = "PDF ファイルが開けませんでした";
            OnPropertyChanged(nameof(Status));
        }
        catch (IOException exception)
        {
            OperationMessage = "ファイルへ保存できませんでした: " + exception.Message;
            OnPropertyChanged(nameof(Status));
        }
    }

    partial void OnPdfPathChanged(string value)
    {
        OperationMessage = null;
    }

    partial void OnPasswordChanged(string value)
    {
        OperationMessage = null;
    }

    private static string NormalizePath(string? path) =>
        path?.Trim().Trim('"') ?? string.Empty;
}
