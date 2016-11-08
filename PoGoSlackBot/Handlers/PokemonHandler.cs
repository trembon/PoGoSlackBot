using NLog;
using PoGoSlackBot.DAL;
using PoGoSlackBot.Entities;
using PoGoSlackBot.Messages.Pokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Handlers
{
    public class PokemonHandler
    {
        internal const int KEEP_NEARBY_HOURS = 4;
        private readonly Logger log;

        private PogoInstance pogoInstance;

        private Dictionary<string, SpawnedPokemon> spawnedEncounters;
        
        private Dictionary<string, DateTime> nearbyEncounters;

        public PokemonHandler(PogoInstance pogoInstance)
        {
            this.pogoInstance = pogoInstance;
            this.log = LogManager.GetLogger($"PokemonHandler ({pogoInstance.Configuration.Name})");

            spawnedEncounters = this.pogoInstance.Database.GetLatestSpawnData();
            nearbyEncounters = this.pogoInstance.Database.GetLatestNearbyData();
        }

        public void HandleSpawnedPokemon(SpawnedPokemon spawnedPokemon)
        {
            if (!spawnedEncounters.ContainsKey(spawnedPokemon.SpawnPointID) || spawnedEncounters[spawnedPokemon.SpawnPointID].EncounterID != spawnedPokemon.EncounterID)
            {
                this.pogoInstance.Database.AddPokemonSpawn(spawnedPokemon);

                log.Info($"Pokemon, {spawnedPokemon.PokemonID.ToString()}, spawned at long:{spawnedPokemon.Longitude},lat:{spawnedPokemon.Latitude}");
                var message = new SpawnedPokemonMessage(spawnedPokemon, pogoInstance.Configuration);
                message.Send();

                spawnedEncounters[spawnedPokemon.SpawnPointID] = spawnedPokemon;
                if (!nearbyEncounters.ContainsKey(spawnedPokemon.EncounterID))
                    nearbyEncounters.Add(spawnedPokemon.EncounterID, spawnedPokemon.Encountered);
            }
            else
            {
                this.pogoInstance.Database.UpdatePokemonSpawn(spawnedPokemon);
            }
        }

        public void HandleNearbyPokemon(NearbyPokemon nearbyPokemon)
        {
            if(!nearbyEncounters.ContainsKey(nearbyPokemon.EncounterID))
            {
                nearbyEncounters.Add(nearbyPokemon.EncounterID, nearbyPokemon.Encountered);
                this.pogoInstance.Database.AddNearbyPokemon(nearbyPokemon);

                log.Info($"Pokemon, {nearbyPokemon.PokemonID.ToString()}, is nearby at long:{this.pogoInstance.Session.Player.Longitude}, lat:{this.pogoInstance.Session.Player.Latitude}");
                var message = new NearbyPokemonMessage(nearbyPokemon, pogoInstance.Configuration);
                message.Send();
            }
        }

        public void CleanNearbyPokemonList()
        {
            DateTime fourHoursAgo = DateTime.Now.AddHours(-KEEP_NEARBY_HOURS);
            foreach (var kvp in nearbyEncounters.Where(kvp => kvp.Value <= fourHoursAgo).ToList())
                nearbyEncounters.Remove(kvp.Key);
        }
    }
}
