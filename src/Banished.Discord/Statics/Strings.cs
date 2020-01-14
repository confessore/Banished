using System.Collections.Generic;

namespace Banished.Discord.Statics
{
    internal static class Strings
    {
        public static string GuildName => "Banished";
        public static string RaidChannel => "📆raid-signups";
        public static string DevChannel => "dev";

        public static List<string> Resources = new List<string>();

        public static string[] Classes =>
            new string[]
            {
                "druid",
                "mage",
                "shaman",
                "warrior",
                "warlock",
                "priest",
                "hunter",
                "rogue"
            };

        public static string[] Reactions =>
            new string[]
            {
                "warriortank",
                "druidbear",
                "shamanresto",
                "priestholy",
                "druidresto",
                "mage",
                "warlock",
                "warriordps",
                "hunter",
                "rogue",
                "shamanelemental",
                "priestshadow",
                "shamanenhancement",
                "druidboomkin",
                "druidcat"
            };
    }
}
