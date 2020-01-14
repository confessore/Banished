using Banished.Net.Data;
using Banished.Net.Models;
using Banished.Net.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Banished.Net.Services
{
    public class DiscordService : IDiscordService
    {
        readonly ApplicationDbContext context;
        readonly UserManager<ApplicationUser> userManager;
        HttpClient httpClient;

        public DiscordService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            HttpClient httpClient)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpClient = httpClient;
        }

        public async Task UpdateUserAsync(ApplicationUser applicationUser)
        {
            var discordUser = await GetUserProfileAsync(applicationUser);
            if (context.DiscordUser.Any(x => x.Id == discordUser.Id))
                context.DiscordUser.Remove(context.DiscordUser.FirstOrDefault(x => x.Id == discordUser.Id));
            applicationUser.UserName = discordUser.Username;
            discordUser.ApplicationUser = applicationUser;
            context.Add(discordUser);
            await context.SaveChangesAsync();
            await userManager.UpdateAsync(applicationUser);
        }

        /*public async Task<List<DiscordConnection>> GetUserConnectionsAsync(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            if (await AccessTokenIsExpiredAsync(userManager, user))
                await RenewAccessTokenAsync(userManager, user);
            var token = await userManager.GetAuthenticationTokenAsync(user, "Discord", "access_token");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var response = await httpClient.GetAsync("https://discordapp.com/api/users/@me/connections");
            return await response.Content.ReadAsAsync<List<DiscordConnection>>();
        }*/

        async Task<DiscordUser> GetUserProfileAsync(ApplicationUser applicationUser)
        {
            if (await AccessTokenIsExpiredAsync(applicationUser))
                await RenewAccessTokenAsync(applicationUser);
            var token = await userManager.GetAuthenticationTokenAsync(applicationUser, "Discord", "access_token");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var response = await httpClient.GetAsync("https://discordapp.com/api/users/@me");
            return await response.Content.ReadAsAsync<DiscordUser>();
        }

        async Task<bool> AccessTokenIsExpiredAsync(ApplicationUser applicationUser) =>
            DateTime.Parse(await userManager.GetAuthenticationTokenAsync(applicationUser, "Discord", "expires_at")).AddDays(-2) < DateTime.Now;

        async Task RenewAccessTokenAsync(ApplicationUser applicationUser)
        {
            var token = await userManager.GetAuthenticationTokenAsync(applicationUser, "Discord", "refresh_token");
            var parameters = new Dictionary<string, string>()
            {
                { "client_id", Environment.GetEnvironmentVariable("StockyDiscordId")},
                { "client_secret", Environment.GetEnvironmentVariable("StockyDiscordSecret") },
                { "grant_type", "refresh_token" },
                { "refresh_token", token },
#if DEBUG
                { "redirect_uri", "https://localhost:44370/signin-discord" },
#else
                { "redirect_uri", "http://nashormatch.com/signin-discord" },
#endif
                { "scope", "identify email connections" }
            };
            httpClient.DefaultRequestHeaders.Clear();
            var response = await httpClient.PostAsync(new Uri("https://discordapp.com/api/oauth2/token"), new FormUrlEncodedContent(parameters));
            if (response.IsSuccessStatusCode)
            {
                var access = await response.Content.ReadAsAsync<DiscordAccessTokenResponse>();
                await userManager.SetAuthenticationTokenAsync(applicationUser, "Discord", "access_token", access.Access_Token);
                await userManager.SetAuthenticationTokenAsync(applicationUser, "Discord", "token_type", access.Token_Type);
                await userManager.SetAuthenticationTokenAsync(applicationUser, "Discord", "expires_at", DateTime.Now.AddSeconds(access.Expires_In).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"));
                await userManager.SetAuthenticationTokenAsync(applicationUser, "Discord", "refresh_token", access.Refresh_Token);
            }
        }
    }
}
