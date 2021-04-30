using System.Collections;
using System.Collections.Generic;

namespace PlanningPoker.Api.Models
{
    public class Room
    {
        public int Id { get; set; }

        public bool Reveal { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
