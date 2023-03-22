using HuTao.NET.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TaoTray.Core
{
    public class ExpeditionModel
    {
        public ImageSource? Icon { get; set; }
        public string? Status { get; set; }
        public int Progress { get; set; }
    }
    internal class ExpeditionModelConverter
    {
        internal static List<ExpeditionModel> Convert(DailyNoteData data)
        {
            List<ExpeditionModel> model = new List<ExpeditionModel>();
            foreach (var ex in data.Expeditions!)
            {
                model.Add(new ExpeditionModel()
                {
                    Icon = new BitmapImage(new Uri(ex.AvatarSideIconUrl)),
                    Status = GetStatus(ex),
                    Progress = GetProgressPercent(int.Parse(ex.RemainedTime))
                });
            }

            return model;
        }

        private static string GetStatus(Expedition ex)
        {
            switch (ex.Status)
            {
                case "Ongoing":
                    return Properties.Resources.STATUS_EXPEDITION_ON_GOING + InfoWindow.ConvertTime(DailyNote.ToTime(ex.RemainedTime).CompleteTime);
                case "Finished":
                    return Properties.Resources.STATUS_EXPEDITION_FINISHED;
                default:
                    return ex.Status;
            }
        }

        private static int GetProgressPercent(int time)
        {
            float percent = (Constant.EXPEDITION_MAX_TIME - time);
            return (int)(percent / Constant.EXPEDITION_MAX_TIME * 100);
        }
    }
}
