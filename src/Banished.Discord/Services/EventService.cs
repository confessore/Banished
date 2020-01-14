using Banished.Discord.Services.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Banished.Discord.Discord.Services
{
    public class EventService : IEventService
    {
        readonly IServiceProvider services;
        readonly DiscordSocketClient client;
        readonly CommandService commandService;
        readonly IBaseService baseService;

        public EventService(
            IServiceProvider services,
            DiscordSocketClient client,
            CommandService commandService,
            IBaseService baseService)
        {
            this.services = services;
            this.client = client;
            this.commandService = commandService;
            this.baseService = baseService;
            client.Ready += Ready;
            client.Disconnected += Disconnected;
            client.MessageReceived += MessageReceived;
            client.UserJoined += UserJoined; 
        }

        async Task Ready()
        {
            await baseService.CheckChannelsAsync();
        }

        Task Disconnected(Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(-1);
            return Task.CompletedTask;
        }

        async Task MessageReceived(SocketMessage msg)
        {
            var tmp = (SocketUserMessage)msg;
            if (tmp == null) return;
            var pos = 0;
            if (!(tmp.HasCharPrefix('>', ref pos) ||
                tmp.HasMentionPrefix(client.CurrentUser, ref pos)) ||
                tmp.Author.IsBot)
                return;
            var context = new SocketCommandContext(client, tmp);
            var result = await commandService.ExecuteAsync(context, pos, services);
            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }

        async Task UserJoined(SocketGuildUser user)
        {
            var dm = await client.GetUser(user.Id).GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync("Welcome to Banished!\n" +
                "Please change your Discord name to reflect your main in-game character name.\n" +
                "When you have done this, type the command '>verify' for your roles to be applied.");
        }
    }
}
