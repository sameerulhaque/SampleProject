using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Utilities.AppSettings
{
    public class AppSetting
    {

        public static int CachedTimeInSeconds
        {
            get { return Convert.ToInt32(AppSetttingHelper.Settting("MemoryCacheSettings:CacheTimeInSeconds") ?? "0"); }
        }
    }
}
