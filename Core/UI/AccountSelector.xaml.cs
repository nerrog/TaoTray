using HuTao.NET.Models;
using System;
using System.Windows;

namespace TaoTray.Core.UI
{
    /// <summary>
    /// AccountSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountSelector : Window
    {
        internal EventHandler<CompletedEventArgs>? Completed;
        private System.Collections.Generic.List<GameRole> Value;
        private bool CloseFlag = false;

        public AccountSelector(GameRoles roles)
        {
            InitializeComponent();
            AccountList.Items.Clear();
            AccountList.ItemsSource = roles.data!.list;
            Value = roles.data!.list;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (AccountList.SelectedItem == null) e.Handled = false;

            int uid = int.Parse(Value[AccountList.SelectedIndex].game_uid);
            Completed?.Invoke(this, new CompletedEventArgs(uid));
            CloseFlag = true;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!CloseFlag) App.Current.Shutdown();
        }
    }

    public class CompletedEventArgs : EventArgs
    {
        public int uid;

        public CompletedEventArgs(int uid)
        {
            this.uid = uid;
        }
    }
}
