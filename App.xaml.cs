using HuTao.NET;
using HuTao.NET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static Cookie? Cookie;
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
            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(AppConfig.Language);
            }
            catch (System.Exception) { }
            
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

    class StartupManager
    {
        private static string getShortcutPath()
        {
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return System.IO.Path.Combine(startupPath, "TaoTray.lnk");
        }

        internal static void createStartupShortcut()
        {
            Type? t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            if (t == null) return;
            dynamic? shell = Activator.CreateInstance(t);
            if (shell == null) return;
            var shortcut = shell.CreateShortcut(getShortcutPath());

            shortcut.Description = "TaoTray";
            shortcut.TargetPath = Assembly.GetExecutingAssembly().Location;
            shortcut.IconLocation = Assembly.GetExecutingAssembly().Location + ",0";
            shortcut.Save();

            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
        }

        internal static void removeStartupShortcut()
        {
            string shortcutPath = getShortcutPath();
            if (System.IO.File.Exists(shortcutPath))
            {
                System.IO.File.Delete(shortcutPath);
            }
        }
    }

}
