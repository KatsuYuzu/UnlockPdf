using Microsoft.Win32;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace UnlockPdf.ViewModels
{
    class MainWindowViewModel
    {

        public MainWindowViewModel()
        {
            // set

            PdfPath = new ReactiveProperty<string>();
            Password = new ReactiveProperty<string>();

            FileExists = PdfPath
                .Select(x => File.Exists(x))
                .ToReactiveProperty();

            HasPassword = Password
                .Select(x => !string.IsNullOrWhiteSpace(x))
                .ToReactiveProperty();

            Status = Observable.CombineLatest(
                    FileExists,
                    HasPassword,
                    (x, y) => !x
                        ? "PDF ファイルを選択してください"
                        : !y
                            ? "パスワードを入力してください"
                            : "")
                .ToReactiveProperty();

            SelectFileCommand = new ReactiveCommand();

            UnlockCommand = Observable.CombineLatest(
                    FileExists,
                    HasPassword,
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
                    if (File.Exists(PdfPath.Value))
                    {
                        var destinationPath =
                            PathHelper.CreateUniquePath(
                                Path.GetDirectoryName(PdfPath.Value),
                                Path.ChangeExtension(PdfPath.Value, null) + "_unlocked.pdf");
                        PdfUnlocker.Unlock(PdfPath.Value, destinationPath, Password.Value);
                        Status.Value = "PDF ファイルのパスワードを解除しました";
                    }
                    else
                    {
                        PdfPath.ForceNotify();
                    }
                });

        }

        public ReactiveProperty<string> PdfPath { get; private set; }
        public ReactiveProperty<string> Password { get; private set; }

        public ReactiveProperty<bool> FileExists { get; private set; }
        public ReactiveProperty<bool> HasPassword { get; private set; }

        public ReactiveProperty<string> Status { get; private set; }

        public ReactiveCommand SelectFileCommand { get; private set; }
        public ReactiveCommand UnlockCommand { get; private set; }

    }
}
