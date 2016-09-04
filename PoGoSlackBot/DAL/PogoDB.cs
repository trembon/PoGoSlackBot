using NLog;
using PoGoSlackBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.DAL
{
    public static class PogoDB
    {
        private static readonly Logger log = LogManager.GetLogger("PogoDB");

        private static object addLock = new object();

        public static void AddPokemonSpawn(SpawnedPokemon spawnedPokemon)
        {
            try
            {
                lock (addLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        context.PokemonSpawns.Add(spawnedPokemon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to add pokemon spawn to database.");
            }
        }
    }
}
