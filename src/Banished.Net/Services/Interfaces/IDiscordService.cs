using Banished.Net.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Banished.Net.Services.Interfaces
{
    public interface IDiscordService
    {
        Task UpdateUserAsync(ApplicationUser applicationUser);
    }
}
