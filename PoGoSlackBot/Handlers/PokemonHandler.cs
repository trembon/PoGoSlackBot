using POGOProtos.Map.Pokemon;
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
        private PogoInstance pogoInstance;

        private HashSet<string> spawnedEncounters;
        private HashSet<string> nearbyEncounters;

        public PokemonHandler(PogoInstance pogoInstance)
        {
            this.pogoInstance = pogoInstance;

            spawnedEncounters = new HashSet<string>();
            nearbyEncounters = new HashSet<string>();
        }

        public void HandleSpawnedPokemon(SpawnedPokemon spawnedPokemon)
        {
            if (!spawnedEncounters.Contains(spawnedPokemon.EncounterID))
            {
                spawnedEncounters.Add(spawnedPokemon.EncounterID);

                PogoDB.AddPokemonSpawn(spawnedPokemon);

                var message = new SpawnedPokemonMessage(spawnedPokemon, nearbyEncounters.Contains(spawnedPokemon.EncounterID), pogoInstance.Configuration);
                message.Send();
            }
        }

        public void HandleNearbyPokemon(NearbyPokemon nearbyPokemon)
        {
            string encounterId = nearbyPokemon.EncounterId.ToString();
            if(!nearbyEncounters.Contains(encounterId) && !spawnedEncounters.Contains(encounterId))
            {
                nearbyEncounters.Add(encounterId);

                var message = new NearbyPokemonMessage(nearbyPokemon, pogoInstance.Configuration);
                message.Send();
            }
        }
    }
}
