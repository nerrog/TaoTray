using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaoTray.Core.Cache
{
    internal class CacheManager
    {

        internal CacheManager()
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CacheDir)))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CacheDir));
            }
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CacheDir + "/", ImagesCacheDir)))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CacheDir + "/", ImagesCacheDir));
            }
        }

        private List<CacheItem<String>> ImageCache = new List<CacheItem<String>>() { 
            new CacheItem<String>(){ ItemID = "DAILY_TASK", ItemRemoteAddres = Constant.HoyoLabUrls.ICON_DAILY_TASK, LocalItemPath = "DailyTask.png"},
            new CacheItem<String>(){ ItemID = "WEEKLY_BOSS", ItemRemoteAddres = Constant.HoyoLabUrls.ICON_WEEKLY_BOSS, LocalItemPath = "WeeklyBoss.png"},
            new CacheItem<String>(){ ItemID = "TRANSFORMER", ItemRemoteAddres = Constant.HoyoLabUrls.ICON_TRANSFORMER, LocalItemPath = "Transformer.png"},
            new CacheItem<String>(){ ItemID = "RESIN", ItemRemoteAddres = Constant.HoyoLabUrls.ICON_RESIN, LocalItemPath = "Resin.png"},
            new CacheItem<String>(){ ItemID = "HOME_COIN", ItemRemoteAddres = Constant.HoyoLabUrls.ICON_HOME_COIN, LocalItemPath = "HomeCoin.png"},
        };


        private string CacheDir = "Cache";
        string ImagesCacheDir = "images";

        internal List<CacheItem<String>> GetImageCache()
        {
            
            var client = new HttpClient();

            Parallel.ForEach(ImageCache, async item =>
            {
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CacheDir + "/", ImagesCacheDir + "/", item.LocalItemPath);

                if (!File.Exists(imagePath))
                {
                    //画像新規取得
                    var response = await client.GetAsync(item.ItemRemoteAddres);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK) throw new HttpRequestException();
                    //保存
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var outStream = File.Create(imagePath);
                    stream.CopyTo(outStream);
                }

                lock (ImageCache)
                {
                    var itemIndex = ImageCache.IndexOf(item);
                    item.Item = imagePath;
                    ImageCache[itemIndex] = item;
                }

            });

            return ImageCache;
        }

        internal static string GetUnknownImagePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unknown.png");
        }
    }


    internal class CacheItem<T>
    {
        internal string ItemID { get; set; } = string.Empty;
        internal string ItemRemoteAddres { get; set; } = string.Empty;
        internal string LocalItemPath { get; set; } = string.Empty;

        internal T? Item { get; set; }
    }
}
