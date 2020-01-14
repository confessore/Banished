using Banished.Discord.Enums;
using System;
using System.Collections.Generic;

namespace Banished.Discord.Models
{
    public class Raid
    {
        public Instance Instance { get; set; }
        public string DateTime { get; set; }
        public IEnumerable<Raider> Raiders { get; set; }
    }
}
