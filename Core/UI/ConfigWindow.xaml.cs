using System.Text.RegularExpressions;
using System.Windows;
using TaoTray.Core.UI;

namespace TaoTray
{
    /// <summary>
    /// ConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            LoadConfigUI();
        }

        private void LoadConfigUI()
        {
            AccountInfo.Text = App.UserData.Role!.NickName + " | " + App.GenshinUser!.uid;
            UpdateCycle.Text = App.AppConfig.UpdateCycle;

            ResinEnable.IsChecked = App.AppConfig.NotifyConfig.ResinRecovery;
            ResinTime.Text = App.AppConfig.NotifyConfig.ResinRecoveryMinutes.ToString();
            HomeCoinEnable.IsChecked = App.AppConfig.NotifyConfig.HomeCoinRecovery;
            HomeCoinTime.Text = App.AppConfig.NotifyConfig.HomeCoinRecoveryMinutes.ToString();
            ExpeditionEnable.IsChecked = App.AppConfig.NotifyConfig.Expedition;
            Transformer.IsChecked = App.AppConfig.NotifyConfig.Transformer;

            DailyEnable.IsChecked = App.AppConfig.NotifyConfig.DailyNotify.Enable;
            DailyTime.Text = App.AppConfig.NotifyConfig.DailyNotify.NotifyTime;
            WeeklyBossEnable.IsChecked = App.AppConfig.NotifyConfig.DailyNotify.WeeklyBoss;
            DailyMissionEnable.IsChecked = App.AppConfig.NotifyConfig.DailyNotify.DailyMission;
        }

        private void Number_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppConfig.UpdateCycle = UpdateCycle.Text;

            App.AppConfig.NotifyConfig.ResinRecovery = (bool)ResinEnable.IsChecked!;
            App.AppConfig.NotifyConfig.ResinRecoveryMinutes = int.Parse(ResinTime.Text);
            App.AppConfig.NotifyConfig.HomeCoinRecovery = (bool)HomeCoinEnable.IsChecked!;
            App.AppConfig.NotifyConfig.HomeCoinRecoveryMinutes = int.Parse(HomeCoinTime.Text);
            App.AppConfig.NotifyConfig.Expedition = (bool)ExpeditionEnable.IsChecked!;
            App.AppConfig.NotifyConfig.Transformer = (bool)Transformer.IsChecked!;

            App.AppConfig.NotifyConfig.DailyNotify.Enable = (bool)DailyEnable.IsChecked!;
            App.AppConfig.NotifyConfig.DailyNotify.NotifyTime = DailyTime.Text;
            App.AppConfig.NotifyConfig.DailyNotify.WeeklyBoss = (bool)WeeklyBossEnable.IsChecked!;
            App.AppConfig.NotifyConfig.DailyNotify.DailyMission = (bool)DailyMissionEnable.IsChecked!;

            App.LoadAndStart();
            this.Close();
        }

        private void AccountChange_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}
