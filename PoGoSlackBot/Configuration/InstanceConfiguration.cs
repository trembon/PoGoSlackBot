using POGOLib.Pokemon.Data;
using PoGoSlackBot.Walking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Configuration
{
    public class InstanceConfiguration
    {
        public string Name { get; set; }

        public LoginProvider LoginProvider { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool ProcessNearbyPokemon { get; set; }

        public bool ProcessGyms { get; set; }

        public bool IgnoreGymUnderAttack { get; set; }

        public string SlackWebHookURL { get; set; }

        public string SlackChannel { get; set; }

        public string SlackBotName { get; set; }

        public Position HomePosition { get; set; }

        public PositionList WalkingPoints { get; set; }

        public MainConfiguration MainConfiguration { get; set; }

        public InstanceConfiguration()
        {
            WalkingPoints = new PositionList();
            HomePosition = null;
        }
    }
}
