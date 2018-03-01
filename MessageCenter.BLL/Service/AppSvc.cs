using MessageCenter.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter.BLL
{
    public class AppSvc
    {
        public static void CreateApp(App app)
        {
            if (AppDA.ExistsApp(app.AppID))
            {
                throw new BusinessException("AppID已经存在！");
            }
            AppDA.CreateApp(app);
        }

        public static IEnumerable<App> LoadApps()
        {
            return AppDA.LoadApps();
        }
    }
}
