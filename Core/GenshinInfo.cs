using HuTao.NET;
using HuTao.NET.Models;
using System;
using System.Threading.Tasks;

namespace TaoTray.Core
{
    internal class GenshinInfo
    {
        private Client client;
        private GenshinUser user;

        internal DailyNoteData data = new DailyNoteData();
        public GenshinInfo(Client client, GenshinUser user)
        {
            this.client = client;
            this.user = user;
        }

        public async Task<DailyNoteData> UpdateInfo()
        {
            var res = await client.FetchDailyNote(user);
            if (res.data != null)
            {
                data = res.data;
                return res.data;
            }
            else
            {
                throw new Exception(Properties.Resources.ERROR_FETCH_DATA);
            }
        }

        public async Task<GenshinStatsData> GetUserData()
        {
            var res = await client.FetchGenshinStats(user);
            if (res.data != null)
            {
                return res.data;
            }
            else
            {
                throw new Exception(Properties.Resources.ERROR_FETCH_USER_DATA);
            }
        }

        public static async Task<int> SearchGameUID(Client client)
        {
            var user = await client.FetchUserStats();

            return GenshinUser.GetUIDFromHoyoLab(user).uid;
        }
    }
}
