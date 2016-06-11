using MahApps.Metro.Controls;
using System.Windows.Input;

namespace UnlockPdf.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            KeyDown += (sender, e) =>
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.W)
                {
                    this.Close();
                }
            };
        }
    }
}
