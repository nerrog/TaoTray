using HuTao.NET.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TaoTray.Core
{
    internal class Timer
    {
        DispatcherTimer timer;
        internal GenshinInfo info;
        private Dispatcher dispatcher;
        private NotifyFlag notifyFlag = new NotifyFlag();

        internal Timer(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            info = new GenshinInfo(App.HuTaoClient!, App.GenshinUser!);

            timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Interval = TimeSpan.Parse(App.AppConfig.UpdateCycle);
            timer.Tick += new EventHandler(TimerEvent);

            //初回
            TimerEvent(this, EventArgs.Empty);

            timer.Start();
        }

        internal void TimerEvent(object? sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (App.AppConfig.LoginLtuid == "" && App.AppConfig.CookieV2.ltoken_v2 == "") return;
                    var res = await info.UpdateInfo();
                    if (res != null)
                    {
                        dispatcher.Invoke(() =>
                        {
                            App.GenshinData = res;
                            App.DataUpdateEvent?.Invoke(this, EventArgs.Empty);
                            TimerChecker();
                            App.LatestUpdate = DateTime.Now;
                        });
                    }
                    else
                    {
                        throw new NullReferenceException(Properties.Resources.ERROR_NULL_DATA);
                    }
                }
                catch (Exception ex)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(Properties.Resources.NOTIFY_ERROR)
                        .AddAttributionText(ex.Message)
                    .Show();
                }
            });
        }

        internal void TimerChecker()
        {
            var config = App.AppConfig.NotifyConfig;
            var data = App.GenshinData;

            //リセット時間を跨いだらフラグをリセット
            TimeSpan timeDiff = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time")) - DateTime.Parse(Constant.GENSHIN_DAILY_RESET_TIME_JST);
            if (timeDiff <= TimeSpan.FromMinutes(Constant.REST_TIME_SPAN_MIN) && timeDiff.TotalMilliseconds >= 0)
            {
                notifyFlag = new NotifyFlag();
            }

            if (config.ResinRecovery)
            {
                if (IsNotify(data.ResinRecoveryTime, config.ResinRecoveryMinutes) && !notifyFlag.Resin)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(string.Format(Properties.Resources.NOTIFY_RESIN_RECOVERY, InfoWindow.ConvertTime(DailyNote.ToTime(data.ResinRecoveryTime).CompleteTime)))
                        .AddAttributionText(data.CurrentResin + "/" + data.MaxResin)
                        .Show();
                    notifyFlag.Resin = true;
                }
            }
            if (config.HomeCoinRecovery)
            {
                if (IsNotify(data.HomeCoinRecoveryTime, config.HomeCoinRecoveryMinutes) && !notifyFlag.HomeCoin)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(string.Format(Properties.Resources.NOTIFY_HOME_COIN_RECOVERY, InfoWindow.ConvertTime(DailyNote.ToTime(data.HomeCoinRecoveryTime).CompleteTime)))
                        .AddAttributionText(data.CurrentHomeCoin + "/" + data.MaxHomeCoin)
                        .Show();
                    notifyFlag.HomeCoin = true;
                }
            }

            if (config.Transformer)
            {
                if (data.Transformer!.RecoveryTime!.Reached && !notifyFlag.Transformer)
                {
                    new ToastContentBuilder()
                        .AddText("TaoTray")
                        .AddText(Properties.Resources.NOTIFY_TRANSFORMER_AVAILABLE)
                        .Show();
                    notifyFlag.Transformer = true;
                }
            }
            if (config.Expedition)
            {
                if (!notifyFlag.Expedition)
                {
                    int finished = 0;
                    foreach (var e in data.Expeditions!)
                    {
                        if (e.Status == "Finished")
                        {
                            finished++;
                        }
                    }
                    if (finished == data.Expeditions.Length)
                    {
                        new ToastContentBuilder()
                            .AddText("TaoTray")
                            .AddText(Properties.Resources.NOTIFY_EXPEDITION_COMPLETED)
                            .Show();
                        notifyFlag.Expedition = true;
                    }
                }
            }
        }

        private bool IsNotify(string RecoveryTime, int min)
        {
            if ((DailyNote.ToTime(RecoveryTime).CompleteTime - DateTime.Now).TotalMinutes <= min)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private class NotifyFlag
        {
            internal bool Resin = false;
            internal bool HomeCoin = false;
            internal bool Transformer = false;
            internal bool Expedition = false;
        }
    }
}
