using System.Windows;

namespace TaoTray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsShow = false;
        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Minimized;
            this.Hide();
            this.ShowInTaskbar = false;
            NotifyIcon.TrayLeftMouseUp += (s, e) => ShowWindow();
#if DEBUG
            NotifyIcon.ToolTipText = "=== DEBUG ===";
#endif
            ShowMainWindow.Click += (s, e) =>
            {
                new ConfigWindow().Show();
            };
            CloseMenuItem.Click += (s, e) =>
            {
                NotifyIcon.CloseBalloon();
            };
            TerminateMenuItem.Click += (s, e) =>
            {
                this.Close(); ;
            };
        }

        public void ShowWindow()
        {
            if (App.AppConfig.LoginLtuid == "") return;
            if (!IsShow)
            {
                InfoWindow info = new InfoWindow();
                NotifyIcon.ShowCustomBalloon(info, System.Windows.Controls.Primitives.PopupAnimation.None, null);
                IsShow = true;
            }
            else
            {
                NotifyIcon.CloseBalloon();
                IsShow = false;
            }

        }

    }
}
