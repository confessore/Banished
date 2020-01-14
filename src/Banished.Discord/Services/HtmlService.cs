using Banished.Discord.Services.Interfaces;
using Banished.Discord.Statics;
using Discord.WebSocket;
using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Banished.Discord.Services
{
    public class HtmlService : IHtmlService
    {
        readonly IBaseService baseService;

        public HtmlService(
            IBaseService baseService)
        {
            this.baseService = baseService;
        }

        HtmlDocument LookupCharacter(string name) =>

            GetHtmlDocumentFromUrl($"http://armory.twinstar.cz/character-sheet.xml?r=Kronos&cn={name}");

        public async Task ModifyClassRoleAsync(SocketGuild guild, SocketGuildUser user, string name)
        {
            try
            {
                var tmp = LookupCharacter(name);
                var @class = GetClass(tmp);
                if (@class != null)
                    await baseService.ModifyRoleAsync(guild, user, @class, true);
            }
            catch { }
        }

        public async Task ModifyClassAndMemberRolesAsync(SocketGuild guild, SocketGuildUser user, string name)
        {
            try
            {
                var tmp = LookupCharacter(name);
                var @class = GetClass(tmp);
                var gld = GetGuild(tmp);
                if (@class != null)
                    await baseService.ModifyRoleAsync(guild, user, @class, true);
                if (gld.ToLower() == Strings.GuildName.ToLower())
                {
                    var role = await baseService.GetGuildRoleAsync(guild, "member");
                    var member = user.Roles.Contains(role);
                    if (!member)
                        await baseService.ModifyRoleAsync(guild, user, role.Name);
                }
            }
            catch { }
        }

        string GetUrl(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            using var response = (HttpWebResponse)request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        HtmlDocument GetHtmlDocumentFromUrl(string url)
        {
            var tmp = new HtmlDocument();
            tmp.LoadHtml(GetUrl(url));
            return tmp;
        }

        string GetError(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//errorhtml").GetAttributeValue("type", string.Empty);

        string GetClass(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//character").GetAttributeValue("class", string.Empty);

        string GetGuild(HtmlDocument document) =>
            document.DocumentNode.SelectSingleNode(@"//character").GetAttributeValue("guildName", string.Empty);
    }
}
