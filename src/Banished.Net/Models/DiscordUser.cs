namespace Banished.Net.Models
{
    public class DiscordUser
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public int Discriminator { get; set; }
        public string Avatar { get; set; }
        public bool Bot { get; set; }
        public bool MFA_Enabled { get; set; }
        public string Locale { get; set; }
        public bool Verified { get; set; }
        public string Email { get; set; }
        public string Flags { get; set; }
        public int Premium_Type { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
