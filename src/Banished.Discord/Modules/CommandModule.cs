using Banished.Discord.Enums;
using Banished.Discord.Services.Interfaces;
using Banished.Discord.Statics;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banished.Discord.Discord.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        readonly IServiceProvider services;
        readonly DiscordSocketClient client;
        readonly CommandService commands;
        readonly IBaseService baseService;
        readonly IHtmlService htmlService;

        public CommandModule(
            IServiceProvider services,
            DiscordSocketClient client,
            CommandService commands,
            IBaseService baseService,
            IHtmlService htmlService)
        {
            this.services = services;
            this.client = client;
            this.commands = commands;
            this.baseService = baseService;
            this.htmlService = htmlService;
        }

        readonly Random random = new Random();

        [Command("help")]
        [Summary("all: displays available commands" +
            "\n >help")]
        async Task HelpAsync()
        {
            await RemoveCommandMessageAsync();
            var embedBuilder = new EmbedBuilder();
            foreach (var command in await commands.GetExecutableCommandsAsync(Context, services))
                embedBuilder.AddField(command.Name, command.Summary ?? "no summary available");
            await ReplyAsync("here's a list of commands and their summaries: ", false, embedBuilder.Build());
        }

        [Command("insult")]
        [Summary("all: got 'em" +
            "\n >insult")]
        async Task InsultAsync()
        {
            await RemoveCommandMessageAsync();
            await ReplyAsync("your mother");
        }

        [Command("nick")]
        [Summary("all: change your nick" +
            "\n >nick 'your nick here'")]
        async Task NickAsync([Remainder] string name)
        {
            await RemoveCommandMessageAsync();
            await client.GetGuild(Context.Guild.Id).GetUser(Context.User.Id).ModifyAsync(x => x.Nickname = name);
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("create")]
        [Summary("officers: creates a new event" +
            "\n >create 'minute' 'hour' 'day' 'month' 'year' 'instance'" +
            "\n >create 0 20 25 10 2019 2" +
            "\n instance: 0 ZG, 1 AQ20, 2 MC, 3 BWL, 4 DoN, 5 AQ40, 6 Naxx")]
        async Task CreateAsync(int minute, int hour, int day, int month, int year, Instance instance)
        {
            await RemoveCommandMessageAsync();
            var dt = DateTime.Parse($"{month}/{day}/{year} {hour}:{minute}").ToString("dd/MMMM/yyyy hh:mm tt 'Server Time'");
            int r;
            string path;
            switch (instance)
            {
                case Instance.ZulGurub:
                    r = random.Next(0, 3);
                    path = Paths.BuildPath(Strings.Resources.FirstOrDefault(x => x.Contains($"ZG{r}")));
                    break;
                case Instance.AhnQirajRuins:
                    r = random.Next(0, 2);
                    path = Paths.BuildPath(Strings.Resources.FirstOrDefault(x => x.Contains($"AQR{r}")));
                    break;
                case Instance.MoltenCore:
                    r = random.Next(0, 3);
                    path = Paths.BuildPath(Strings.Resources.FirstOrDefault(x => x.Contains($"MC{r}")));
                    break;
                case Instance.Onyxia:
                    r = random.Next(0, 2);
                    path = Paths.BuildPath(Strings.Resources.FirstOrDefault(x => x.Contains($"ONY{r}")));
                    break;
                case Instance.BlackwingLair:
                    path = Paths.BuildPath(Strings.Resources.FirstOrDefault(x => x.Contains($"BWL")));
                    break;
                default:
                    path = string.Empty;
                    break;
            }
            var reactions = new StringBuilder();
            foreach (var reaction in Strings.Reactions)
                reactions.Append(await baseService.GetEmoteAsync(reaction));
            var msg = await Context.Channel.SendFileAsync(path, $"{instance} {dt} {reactions}");
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("parse")]
        [Summary("officers: parses the raiders that have signed up for an event" +
            "\n >parse 'message id'" +
            "\n >parse 656189757323345950")]
        async Task ParseAsync(ulong id)
        {
            await RemoveCommandMessageAsync();
            var raiders = await baseService.GetRaidersAsync(await baseService.GetRaidChannelUserMessageAsync(id));
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{raiders.Count()} people have signed up for this event: \n");
            foreach (var raider in raiders)
                stringBuilder.Append($"{raider.Name} {raider.Role}\n");
            await ReplyAsync(stringBuilder.ToString());
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("update")]
        [Summary("officers: updates each guild users class role according to their nickname (character name)" +
            "\n >update" +
            "\n >update")]
        async Task UpdateAsync()
        {
            await RemoveCommandMessageAsync();
            foreach (var user in Context.Guild.Users.Where(x => x.Status == UserStatus.Online))
                await htmlService.ModifyClassRoleAsync(Context.Guild, user, user.Nickname ?? user.Username);
        }

        [Command("verify")]
        [Summary("all: updates a single guild user's class role and membership role according to their nickname (character name)" +
            "\n >verify" +
            "\n >verify")]
        async Task VerifyAsync()
        {
            await RemoveCommandMessageAsync();
            var user = client.GetGuild(Context.Guild.Id).GetUser(Context.User.Id);
            await htmlService.ModifyClassAndMemberRolesAsync(Context.Guild, user, user.Nickname ?? user.Username);
        }

        async Task RemoveCommandMessageAsync() =>
            await client.GetGuild(Context.Guild.Id).GetTextChannel(Context.Message.Channel.Id).DeleteMessageAsync(Context.Message);
    }
}
