using Microsoft.Toolkit.Uwp.Notifications;
using System;
using TaoTray.Core.Config;

namespace TaoTray.Core
{
    internal class DailyTimer
    {
        private System.Timers.Timer? timer;
        internal DailyTimer(DateTime time)
        {
            SetTime(time);
        }

        private void SetTime(DateTime time)
        {
            TimeSpan TimeDiff;
            TimeDiff = time - DateTime.Now;
            if (TimeDiff.TotalMilliseconds < 0) TimeDiff = time.AddDays(1) - DateTime.Now;
            timer = new System.Timers.Timer(TimeDiff.TotalMilliseconds);
            timer.Elapsed += TimerEvent;
            timer.Start();
        }

        internal void TimerEvent(object? sender, EventArgs e)
        {
            DailyNotify config = App.AppConfig.NotifyConfig.DailyNotify;

            if (!config.Enable) return;

            if (config.DailyMission)
            {
                if (App.GenshinData.FinishedTaskNum != App.GenshinData.ToatalTaskNum)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(Properties.Resources.NOTIFY_ALERT_DAILY)
                        .Show();
                }
                if (!App.GenshinData.IsExtraTaskRewardRecevied)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(Properties.Resources.NOTIFY_ALERT_DAILY_EXTRA_REWARD)
                        .Show();
                }
            }
            if (config.WeeklyBoss)
            {
                if (App.GenshinData.RemainResinDiscountNum > 0)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(Properties.Resources.NOTIFY_ALERT_WEEKLY_BOSS)
                        .Show();
                }
            }

            var target = DateTime.Parse(App.AppConfig.NotifyConfig.DailyNotify.NotifyTime).AddDays(1);
            //タイマーの再セット
            SetTime(target);
        }
    }
}
