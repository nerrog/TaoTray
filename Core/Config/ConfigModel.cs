namespace TaoTray.Core.Config
{
    public class ConfigModel
    {
        public int ConfigVersion { get; set; } = 1;
        public string LoginLtoken { get; set; } = "";
        public string LoginLtuid { get; set; } = "";
        public int InGameUid { get; set; } = 0;
        public string Language { get; set; } = "ja-jp";
        public string UpdateCycle { get; set; } = "00:08:00";
        public NotifyConfig NotifyConfig { get; set; } = new NotifyConfig();
    }

    public class NotifyConfig
    {
        public bool ResinRecovery { get; set; } = true;
        public int ResinRecoveryMinutes { get; set; } = 20;
        public bool HomeCoinRecovery { get; set; } = true;
        public int HomeCoinRecoveryMinutes { get; set; } = 20;
        public bool Expedition { get; set; } = true;
        public bool Transformer { get; set; } = true;
        public DailyNotify DailyNotify { get; set; } = new DailyNotify();
    }

    public class DailyNotify
    {
        public bool Enable { get; set; } = true;
        public string NotifyTime { get; set; } = "23:00";
        public bool WeeklyBoss { get; set; } = true;
        public bool DailyMission { get; set; } = true;
    }
}
