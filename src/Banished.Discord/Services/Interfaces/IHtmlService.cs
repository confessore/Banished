using Discord.WebSocket;
using System.Threading.Tasks;

namespace Banished.Discord.Services.Interfaces
{
    public interface IHtmlService
    {
        Task ModifyClassRoleAsync(SocketGuild guild, SocketGuildUser user, string name);
        Task ModifyClassAndMemberRolesAsync(SocketGuild guild, SocketGuildUser user, string name);
    }
}
