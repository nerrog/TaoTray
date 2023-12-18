using HuTao.NET;
using HuTao.NET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TaoTray.Core;
using TaoTray.Core.Cache;
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

        public static ICookie? Cookie;
        public static GenshinUser? GenshinUser;
        public static ClientData? ClientData;
        public static Client? HuTaoClient;
        internal static Core.Timer? timer;
        internal static DailyTimer? dailyTimer;

        private static Dispatcher? dispatcher;

        internal static List<CacheItem<String>>? imageCache;

        public App()
        {
            dispatcher = this.Dispatcher;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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
            //画像キャッシュ読み込み
            imageCache = new CacheManager().GetImageCache();

            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(AppConfig.Language);
            }
            catch (Exception) { }
            
            //ログインしていない場合はログインさせる
            if (AppConfig.LoginLtoken == "")
            {
                if (AppConfig.CookieV2.ltoken_v2 == "")
                {
                    new LoginWindow().Show();
                    return;
                }
            }

            //UIDを取得していない場合もログイン画面にリダイレクト
            if (AppConfig.InGameUid == 0)
            {
                new LoginWindow().Show();
                return;
            }


            if (AppConfig.LoginLtoken != "")
            {
                Cookie = new Cookie()
                {
                    ltoken = AppConfig.LoginLtoken,
                    ltuid = AppConfig.LoginLtuid,
                };
            }
            else
            {
                Cookie = AppConfig.CookieV2;
            }

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

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as System.Exception;
#if DEBUG
            //デバッガーに投げる
            if (exception != null) throw exception;
#endif


            var message = $"予期せぬエラーが発生しました。続けて発生する場合は開発者に報告してください。";
            if (exception != null)
            {
                message += $"\n({exception.Message} @ {exception.TargetSite?.Name})";

                var LogMessage = "=========== BEGIN ERROR LOG ===========\n";
                LogMessage += $"Catch UnhandledException at {DateTime.Now}\n";
                LogMessage += exception.ToString()+"\n" ;
                LogMessage += exception.StackTrace + "\n";
                LogMessage += "=========== END ERROR LOG ===========\n";
                LogMessage += "\n";
                File.AppendAllText("error.log", LogMessage);
            }
            MessageBox.Show(message, "未処理例外", MessageBoxButton.OK, MessageBoxImage.Stop);
            Environment.Exit(1);
        }
    }
}
