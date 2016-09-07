using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using PoGoSlackBot.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Entities
{
    [Table("gyms")]
    public class GymDetails
    {
        [Key]
        [Column("gym_id")]
        public string GymID { get; set; }

        [Column("instance")]
        public string InstanceName { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("image_url")]
        public string ImageURL { get; set; }

        [Column("owner")]
        public TeamColor Owner { get; set; }

        [Column("strongest_pokemon")]
        public PokemonId StrongestPokemon { get; set; }

        [Column("is_in_battle")]
        public bool IsInBattle { get; set; }

        [Column("latitude")]
        public double Latitude { get; set; }

        [Column("longitude")]
        public double Longitude { get; set; }

        [Column("last_update")]
        public DateTime LastUpdate { get; set; }

        public GymDetails()
        {
        }

        public GymDetails(FortData gym, string name, string imageUrl, string instanceName)
        {
            this.InstanceName = instanceName;
            this.GymID = gym.Id;

            this.Name = name;
            this.ImageURL = imageUrl;

            this.Update(gym);
        }

        public void Update(FortData gym)
        {
            this.Latitude = gym.Latitude;
            this.Longitude = gym.Longitude;

            this.Owner = gym.OwnedByTeam;
            this.StrongestPokemon = gym.GuardPokemonId;

            this.LastUpdate = DateTime.Now;
        }
    }
}
