using HuTao.NET.Models;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TaoTray.Core;

namespace TaoTray
{
    /// <summary>
    /// InfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InfoWindow : UserControl
    {
        public InfoWindow()
        {
            InitializeComponent();

            App.DataUpdateEvent += (s, e) => DataUpdated(App.GenshinData, App.UserData);
            //初回表示時
            DataUpdated(App.GenshinData, App.UserData);

        }

        internal void DataUpdated(DailyNoteData data, GenshinStatsData user)
        {
            //UserInfo
            UserInfoText.Text = user.Role!.NickName + " | " + "Lv." + user.Role.Level;
            LastUpdatedText.Text = string.Format(Properties.Resources.UI_INFO_LAST_UPDATED, App.LatestUpdate.ToString());
            //Resin
            ResinIcon.Source = new BitmapImage(new Uri(Constant.HoyoLabUrls.ICON_RESIN));
            ResinText.Text = data.CurrentResin + "/" + data.MaxResin;
            var ResinRecoveryDateTime = DailyNote.ToTime(data.ResinRecoveryTime).CompleteTime;
            ResinRecoveryTime.Text = " " + ConvertTime(ResinRecoveryDateTime);
            ResinRecoveryTimeETA.Text = " " + GetETA(ResinRecoveryDateTime);

            Expeditions.ItemsSource = ExpeditionModelConverter.Convert(data);

            DailyImage.Source = new BitmapImage(new Uri(Constant.HoyoLabUrls.ICON_DAILY_TASK));
            DailyNum.Text = " " + data.FinishedTaskNum + "/" + data.ToatalTaskNum;
            WeeklyBossImage.Source = new BitmapImage(new Uri(Constant.HoyoLabUrls.ICON_WEEKLY_BOSS));
            WeeklyBossNum.Text = " " + data.RemainResinDiscountNum + "/ " + data.ResionDiscountNumLimit;
            HomeCoinImage.Source = new BitmapImage(new Uri(Constant.HoyoLabUrls.ICON_HOME_COIN));
            HomeCoinNum.Text = " " + data.CurrentHomeCoin + "/" + data.MaxHomeCoin;
            HomeCoinNum.ToolTip = string.Format(Properties.Resources.UI_INFO_HOME_COIN_RECOVERY, ConvertTime(DailyNote.ToTime(data.HomeCoinRecoveryTime).CompleteTime));
            TransformerImage.Source = new BitmapImage(new Uri(Constant.HoyoLabUrls.ICON_TRANSFORMER));
            TransformerETA.Text = " " + GetTransformerTime(data.Transformer!.RecoveryTime!);
        }

        private string GetETA(DateTime dt)
        {
            if (DateTime.Today.Day == dt.Day)
            {
                return Properties.Resources.TIME_TODAY + dt.ToString("HH:mm");
            }
            else if (DateTime.Today.AddDays(1).Day == dt.Day)
            {
                return Properties.Resources.TIME_TOMORROW + dt.ToString("HH:mm");
            }
            else
            {
                return dt.ToString();
            }
        }

        internal static string ConvertTime(DateTime time)
        {
            return string.Format(Properties.Resources.TIME_ETA, (time - DateTime.Now).Hours, (time - DateTime.Now).Minutes);
        }

        private string GetTransformerTime(TransformerRecoveryTime time)
        {
            if (time.Reached)
            {
                return Properties.Resources.STATUS_AVAILABLE;
            }
            else
            {
                string day = time.Day != 0 ? time.Day + Properties.Resources.TIME_DAY : "";
                string hour = time.Hour != 0 ? time.Hour + Properties.Resources.TIME_HOUR : "";
                string min = time.Minute != 0 ? time.Hour + Properties.Resources.TIME_MINUTE : "";
                return day + hour + min;
            }

        }
    }
}
