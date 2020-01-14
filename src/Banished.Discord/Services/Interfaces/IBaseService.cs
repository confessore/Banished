using Banished.Discord.Models;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Banished.Discord.Services.Interfaces
{
    public interface IBaseService
    {
        Task CheckChannelsAsync();
        Task<IEmote> GetEmoteAsync(string name);
        Task<SocketRole> GetGuildRoleAsync(SocketGuild guild, string name);
        Task<IUserMessage> GetRaidChannelUserMessageAsync(ulong id);
        Task<IEnumerable<Raider>> GetRaidersAsync(IUserMessage message);
        Task<IEnumerable<Raid>> GetRaidsAsync();
        Task ModifyRoleAsync(SocketGuild guild, SocketGuildUser user, string name, bool removeClass = false);
        Task<string> ParseRoleAsync(string reaction);
        ISocketMessageChannel RaidChannel { get; }
        ISocketMessageChannel DevChannel { get; }
        SocketGuild Guild { get; }
    }
}
