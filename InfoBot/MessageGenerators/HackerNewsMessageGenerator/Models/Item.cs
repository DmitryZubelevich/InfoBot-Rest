using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoBot.MessageGenerators.HackerNewsMessageGenerator.Models
{
    public class Item
    {
        public long id { get; set; }

        public bool deleted { get; set; }

        public ItemType type { get; set; }

        public string url { get; set; }

        public string title { get; set; }
    }
}
