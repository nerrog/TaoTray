using HuTao.NET;
using HuTao.NET.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TaoTray.Core;
using TaoTray.Core.Config;
using TaoTray.Core.UI;

namespace TaoTray
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DailyNoteData GenshinData = new DailyNoteData();
        public static GenshinStatsData UserData = new GenshinStatsData();
        public static DateTime LatestUpdate = DateTime.Now;
        internal static ConfigModel AppConfig = new ConfigModel();
        internal static ConfigManager configManager = new ConfigManager();
        internal static EventHandler? DataUpdateEvent;

        public static Cookie? Cookie;
        public static GenshinUser? GenshinUser;
        public static ClientData? ClientData;
        public static Client? HuTaoClient;
        internal static Core.Timer? timer;
        internal static DailyTimer? dailyTimer;

        private static Dispatcher? dispatcher;

        public App()
        {
            dispatcher = this.Dispatcher;
            try
            {
                configManager.GetConfig();
                LoadAndStart();
            }
            catch (FileNotFoundException)
            {
                new LoginWindow().Show();
            }

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            configManager.SaveConfig();
        }

        internal static void LoadAndStart()
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(AppConfig.Language);
            }
            catch (Exception) { }
            
            if (AppConfig.LoginLtoken == "")
            {
                new LoginWindow().Show();
                return;
            }
            Cookie = new Cookie()
            {
                ltoken = AppConfig.LoginLtoken,
                ltuid = AppConfig.LoginLtuid,
            };
            ClientData = new ClientData() { Language = AppConfig.Language };
            HuTaoClient = new Client(Cookie, ClientData);
            GenshinUser = new GenshinUser(AppConfig.InGameUid);

            timer = new Core.Timer(dispatcher!);
            dailyTimer = new DailyTimer(DateTime.Parse(AppConfig.NotifyConfig.DailyNotify.NotifyTime));
            Task.Run(async () =>
            {
                App.UserData = await App.timer.info.GetUserData();
            });
            configManager.SaveConfig(AppConfig);
        }
    }
}
