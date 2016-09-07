using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Entities
{
    [Table("pokemon_nearby")]
    public class NearbyPokemon
    {
        [Key]
        [Column("encounter_id")]
        public string EncounterID { get; set; }

        [Column("instance")]
        public string InstanceName { get; set; }

        [Column("pokemon_id")]
        public PokemonId PokemonID { get; set; }

        [Column("scan_latitude")]
        public double ScanLatitude { get; set; }

        [Column("scan_longitude")]
        public double ScanLongitude { get; set; }

        [Column("encountered")]
        public DateTime Encountered { get; set; }

        public NearbyPokemon()
        {
        }

        public NearbyPokemon(POGOProtos.Map.Pokemon.NearbyPokemon nearbyPokemon, string instanceName, double latitude, double longitude)
        {
            this.EncounterID = nearbyPokemon.EncounterId.ToString();
            this.InstanceName = instanceName;
            this.PokemonID = nearbyPokemon.PokemonId;

            this.ScanLatitude = latitude;
            this.ScanLongitude = longitude;

            this.Encountered = DateTime.Now;
        }
    }
}
