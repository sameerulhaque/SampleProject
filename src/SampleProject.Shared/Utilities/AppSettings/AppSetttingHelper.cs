using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Utilities.AppSettings
{
    public class AppSetttingHelper
    {
        private static IConfiguration _cofiguration = null!;

        public static void AppSettingConfigure(IConfiguration cofiguration)
        {
            _cofiguration = cofiguration;
        }

        public static string? Settting(string key)
        {
            return _cofiguration.GetSection(key).Value;
        }
    }
}
