using System.IO;

namespace Banished.Discord.Statics
{
    internal static class Paths
    {
        public static string BuildPath(string resourceName) => $"{Directory.GetCurrentDirectory()}//{resourceName.Replace('_', '.')}";
    }
}
