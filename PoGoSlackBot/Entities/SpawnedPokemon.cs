using POGOProtos.Enums;
using POGOProtos.Map.Pokemon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Entities
{
    [Table("pokemon_spawns")]
    public class SpawnedPokemon
    {
        [Key]
        [Column("encounter_id")]
        public string EncounterID { get; set; }

        [Column("instance")]
        public string InstanceName { get; set; }

        [Column("spawn_point_id")]
        public string SpawnPointID { get; set; }

        [Column("pokemon_id")]
        public PokemonId PokemonID { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

        [Column("despawn")]
        public DateTime Despawn { get; set; }

        [Column("encountered")]
        public DateTime Encountered { get; set; }

        public SpawnedPokemon()
        {
            this.Encountered = DateTime.Now;
        }

        public SpawnedPokemon(string instanceName) : this()
        {
            this.InstanceName = instanceName;
        }

        public SpawnedPokemon(WildPokemon wildPokemon, string instanceName) : this(instanceName)
        {
            this.PokemonID = wildPokemon.PokemonData.PokemonId;
            this.EncounterID = wildPokemon.EncounterId.ToString();
            this.SpawnPointID = wildPokemon.SpawnPointId;

            this.Latitude = wildPokemon.Latitude;
            this.Longitude = wildPokemon.Longitude;

            this.Despawn = DateTime.Now.AddMilliseconds(wildPokemon.TimeTillHiddenMs);
        }

        public SpawnedPokemon(MapPokemon mapPokemon, string instanceName) : this(instanceName)
        {
            this.PokemonID = mapPokemon.PokemonId;
            this.EncounterID = mapPokemon.EncounterId.ToString();
            this.SpawnPointID = mapPokemon.SpawnPointId;

            this.Latitude = mapPokemon.Latitude;
            this.Longitude = mapPokemon.Longitude;

            DateTime utcStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            this.Despawn = utcStart.AddMilliseconds(mapPokemon.ExpirationTimestampMs).ToLocalTime();
        }
    }
}
