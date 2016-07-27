using iTextSharp.text.exceptions;
using Microsoft.Win32;
using Reactive.Bindings;
using System;
using System.IO;
using System.Reactive.Linq;

namespace UnlockPdf.ViewModels
{
    class MainWindowViewModel
    {
        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public MainWindowViewModel()
        {
            // set

            PdfPath = new ReactiveProperty<string>();
            Password = new ReactiveProperty<string>();

            var isExistsPdf = PdfPath
                .Select(x => x ?? string.Empty)
                .Select(x => x.Trim('"'))
                .Select(x => File.Exists(x));

            var hasPassword = Password
                .Select(x => !string.IsNullOrWhiteSpace(x));

            Status = Observable.CombineLatest(
                    isExistsPdf,
                    hasPassword,
                    (x, y) => !x
                        ? "PDF ファイルを選択してください"
                        : !y
                            ? "パスワードを入力してください"
                            : "[ENTER] キーまたは [UNLOCK] ボタンを押してください")
                .ToReactiveProperty();

            SelectFileCommand = new ReactiveCommand();

            UnlockCommand = Observable.CombineLatest(
                    isExistsPdf,
                    hasPassword,
                    (x, y) => x && y)
                .ToReactiveCommand();

            // subscribe

            SelectFileCommand
                .Subscribe(_ =>
                {
                    var dialog = new OpenFileDialog()
                    {
                        Filter = "PDF (*.pdf)|*.pdf"
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        PdfPath.Value = dialog.FileName;
                    }
                });

            UnlockCommand
                .Subscribe(_ =>
                {
                    var trimmedPdfPath = PdfPath.Value.Trim('"');

                    if (File.Exists(trimmedPdfPath))
                    {
                        var destinationPath =
                            PathHelper.CreateUniquePath(
                                Path.GetDirectoryName(trimmedPdfPath),
                                Path.ChangeExtension(trimmedPdfPath, null) + "_unlocked.pdf");
                        try
                        {
                            PdfUnlocker.Unlock(trimmedPdfPath, destinationPath, Password.Value);
                            Password.Value = null;
                            Status.Value = "PDF ファイルのパスワードを解除しました";
                            PathHelper.OpenExplorerWithSelected(destinationPath);
                        }
                        catch (BadPasswordException)
                        {
                            Password.Value = null;
                            Status.Value = "正しいパスワードを入力してください";
                        }
                        catch (InvalidPdfException)
                        {
                            Status.Value = "PDF ファイルが開けませんでした";
                        }
                    }
                    else
                    {
                        PdfPath.ForceNotify();
                    }
                });

        }

        public ReactiveProperty<string> PdfPath { get; private set; }
        public ReactiveProperty<string> Password { get; private set; }

        public ReactiveProperty<string> Status { get; private set; }

        public ReactiveCommand SelectFileCommand { get; private set; }
        public ReactiveCommand UnlockCommand { get; private set; }

    }
}
