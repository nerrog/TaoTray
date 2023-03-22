using HuTao.NET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Windows;

namespace TaoTray.Core.UI
{
    /// <summary>
    /// LoginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool CloseFlag = false;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Cookie c = new Cookie();
            var cookies = await webView2.CoreWebView2.CookieManager.GetCookiesAsync("https://www.hoyolab.com");
            foreach (var cookie in cookies)
            {
                if (cookie.Name == "ltuid") c.ltuid = cookie.Value;
                if (cookie.Name == "ltoken") c.ltoken = cookie.Value;
            }

            if (c.ltoken == "" || c.ltuid == "")
            {
                MessageBox.Show(Properties.Resources.UI_LOGIN_NOT_LOGGED_IN, Properties.Resources.UI_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            App.AppConfig.LoginLtuid = c.ltuid;
            App.AppConfig.LoginLtoken = c.ltoken;

            var roles = await new Client(c).GetGenshinRoles();

            if (roles.data!.list.Count == 0)
            {
                MessageBox.Show(Properties.Resources.UI_LOGIN_ACCOUNT_NOT_FOUND, Properties.Resources.UI_ERROR);
                App.Current.Shutdown();
            }
            else if (roles.data!.list.Count == 1)
            {
                AccountSelected(this, new CompletedEventArgs(int.Parse(roles.data!.list[0].game_uid)));
            }
            else
            {
                var selecter = new AccountSelector(roles);
                selecter.Completed += AccountSelected;
                OK_Button.IsEnabled = false;
                selecter.Show();
            }
        }

        private void AccountSelected(object? sender, CompletedEventArgs e)
        {
            App.AppConfig.InGameUid = e.uid;

            App.LoadAndStart();
            new ToastContentBuilder()
                .AddText("TaoTray")
                .AddText(Properties.Resources.NOTIFY_LOGIN_SUCCESSFUL)
                .AddAttributionText(Properties.Resources.NOTIFY_LOGIN_SUCCESSFUL_DESC)
                .Show();

            CloseFlag = true;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!CloseFlag) App.Current?.Shutdown();
        }
    }
}
