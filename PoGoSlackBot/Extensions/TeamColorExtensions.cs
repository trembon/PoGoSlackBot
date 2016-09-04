using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Extensions
{
    public static class TeamColorExtensions
    {
        public static string ToTeamName(this TeamColor teamColor)
        {
            switch (teamColor)
            {
                case TeamColor.Blue: return "Mystic";
                case TeamColor.Red: return "Valor";
                case TeamColor.Yellow: return "Instinct";
            }
            return "Neutral";
        }
    }
}
