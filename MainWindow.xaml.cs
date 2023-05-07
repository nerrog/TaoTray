using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

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
            DataUpdate.Click += (s, e) =>
            {
                if (App.timer != null)
                {
                    App.timer.TimerEvent(this, System.EventArgs.Empty);
                }
            };



            //カスタムアイコン
            string CustomIconPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "icon.png");
            if (File.Exists(CustomIconPath))
            {
                Image img = Image.FromFile(CustomIconPath);

                Bitmap bitmap = new Bitmap(64, 64, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bitmap);
                g.DrawImage(img, new Rectangle(0, 0, 64, 64));
                g.Dispose();

                //ビットマップをアイコンへ変換
                Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());

                NotifyIcon.Icon = icon;

                //後始末
                bitmap.Dispose();
                icon.Dispose();
                img.Dispose();
            }

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
