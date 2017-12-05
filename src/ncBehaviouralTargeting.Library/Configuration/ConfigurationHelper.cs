using System.Configuration;

namespace ncBehaviouralTargeting.Library.Configuration
{

    internal static class ConfigurationHelper
    {
        public static FootprintConfigurationSection Settings => ConfigurationManager.GetSection("footprintConfiguration") as FootprintConfigurationSection;
    }
}
