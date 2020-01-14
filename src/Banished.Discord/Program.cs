using Banished.Discord.Discord.Services;
using Banished.Discord.Services;
using Banished.Discord.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Banished.Discord
{
    class Program
    {
        readonly IServiceProvider services;
        readonly DiscordSocketClient client;

        Program()
        {
            client = new DiscordSocketClient();
            services = ConfigureServices();
        }

        static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        async Task MainAsync()
        {
            await services.GetRequiredService<IRegistrationService>().IntializeRegistrationsAsync();
            await client.LoginAsync(
                TokenType.Bot,
                Environment.GetEnvironmentVariable("BanishedDiscordToken"));
            await client.StartAsync();
            await client.SetGameAsync("'>help' for commands");
            await Task.Delay(-1);
        }

        IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<CommandService>()
                .AddSingleton<IBaseService, BaseService>()
                .AddSingleton<IEventService, EventService>()
                .AddSingleton<IHtmlService, HtmlService>()
                .AddSingleton<IRegistrationService, RegistrationService>()
                .BuildServiceProvider();
        }
    }
}
