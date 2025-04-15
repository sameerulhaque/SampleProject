using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure.Caching
{
    public static class CacheHelper
    {
        public static TimeSpan GetCacheExpirationTimeSpan(int seconds)
        {
            int h, m, s;
            h = m = 0;

            if (seconds >= 60)
            {
                m = seconds / 60;

                if (m >= 60)
                {
                    h = m / 60;
                    m %= 60;
                }

                s = seconds % 60;
            }
            else
            {
                s = seconds;
            }

            return new TimeSpan(h, m, s);
        }
    }
}
