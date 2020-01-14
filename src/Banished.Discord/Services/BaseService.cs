using Banished.Discord.Enums;
using Banished.Discord.Models;
using Banished.Discord.Services.Interfaces;
using Banished.Discord.Statics;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banished.Discord.Services
{
    public class BaseService : IBaseService
    {
        readonly DiscordSocketClient client;

        public BaseService(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task CheckChannelsAsync()
        {
            if (!Guild.TextChannels.Any(x => x.Name == Strings.DevChannel))
                await Guild.CreateTextChannelAsync(Strings.DevChannel);
        }

        public Task<IEmote> GetEmoteAsync(string name) =>
            Task.FromResult((IEmote)Guild.Emotes.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault());

        public Task<SocketRole> GetGuildRoleAsync(SocketGuild guild, string name) =>
            Task.FromResult(client.GetGuild(guild.Id).Roles.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()));

        public async Task<IUserMessage> GetRaidChannelUserMessageAsync(ulong id)
        {
            var tmp = await RaidChannel.GetMessagesAsync().FlattenAsync();
                return (IUserMessage)tmp.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<Raider>> GetRaidersAsync(IUserMessage message)
        {
            var raiders = new List<Raider>();
            foreach (var rctn in message.Reactions)
            {
                foreach (var user in await message.GetReactionUsersAsync(rctn.Key, 1000).FirstOrDefault())
                {
                    if (user.Id != client.CurrentUser.Id)
                    {
                        if (!Strings.Reactions.Contains(rctn.Key.Name.ToLower()))
                        {
                            await message.RemoveReactionAsync(rctn.Key, user);
                            continue;
                        }
                        var role = await ParseRoleAsync(rctn.Key.Name);
                        if (Strings.Classes.Contains(role))
                        {
                            var tmp = Guild.Users.FirstOrDefault(x => x.Id == user.Id);
                            var raider = new Raider
                            {
                                User = user,
                                Name = tmp.Nickname ?? tmp.Username,
                                Role = rctn.Key
                            };
                            var existing = raiders.Any(x => x.Name == raider.Name);
                            raiders.Add(raider);
                        }
                    }
                }
            }
            return raiders;
        }

        public async Task<IEnumerable<Raid>> GetRaidsAsync()
        {
            IEnumerable<IMessage> messages = await RaidChannel.GetMessagesAsync().FlattenAsync();
            var raids = new List<Raid>();
            foreach (var message in messages)
            {
                if (message.Author == client.CurrentUser)
                {
                    var content = message.Content.Split(' ');
                    var raid = new Raid()
                    {
                        Instance = Enum.Parse<Instance>(content[0]),
                        DateTime = content[1],
                        Raiders = await GetRaidersAsync((IUserMessage)message)
                    };
                    raids.Add(raid);
                }
            }
            return raids;
        }

        public async Task ModifyRoleAsync(SocketGuild guild, SocketGuildUser user, string name, bool removeClass = false)
        {
            var roles = client.GetGuild(guild.Id).GetUser(user.Id).Roles;
            if (removeClass)
            {
                foreach (var role in roles.Where(x => Strings.Classes.Any(y => x.ToString().ToLower() == y.ToLower())))
                    await client.GetGuild(guild.Id).GetUser(user.Id).RemoveRoleAsync(role);
            }
            await client.GetGuild(guild.Id).GetUser(user.Id).AddRoleAsync(await GetGuildRoleAsync(guild, name));
        }

        public Task<string> ParseRoleAsync(string reaction)
        {
            return Task.FromResult((reaction.ToLower()) switch
            {
                "warriortank" => "warrior",
                "druidbear" => "druid",
                "shamanresto" => "shaman",
                "priestholy" => "priest",
                "druidresto" => "druid",
                "mage" => "mage",
                "warlock" => "warlock",
                "warriordps" => "warrior",
                "hunter" => "hunter",
                "rogue" => "rogue",
                "shamanelemental" => "shaman",
                "priestshadow" => "priest",
                "shamanenhancement" => "shaman",
                "druidboomkin" => "druid",
                "druidcat" => "druid",
                _ => string.Empty,
            });
        }

        public ISocketMessageChannel RaidChannel =>
            Guild.TextChannels.FirstOrDefault(x => x.Name == Strings.RaidChannel);
        public ISocketMessageChannel DevChannel =>
            Guild.TextChannels.FirstOrDefault(x => x.Name == Strings.DevChannel);
        public SocketGuild Guild =>
            client.Guilds.FirstOrDefault(x => x.Name == Strings.GuildName);
    }
}
