using POGOLib.Net;
using POGOLib.Pokemon;
using POGOProtos.Map.Fort;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.DAL;
using PoGoSlackBot.Entities;
using PoGoSlackBot.Handlers;
using PoGoSlackBot.Walking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Handlers
{
    public class PogoInstance
    {
        public Session Session { get; }

        public PokemonHandler PokemonHandler { get; }

        public GymHandler GymHandler { get; }

        public Walker Walker { get; }

        public InstanceConfiguration Configuration { get; }

        public PogoDB Database { get; }

        public PogoInstance(InstanceConfiguration configuration, Session session, Walker walker)
        {
            this.Configuration = configuration;
            this.Session = session;

            this.Database = new PogoDB(this.Configuration.Name);

            this.Walker = walker;
            this.GymHandler = new GymHandler(this);
            this.PokemonHandler = new PokemonHandler(this);

            this.Session.Map.Update += Map_Update;
        }

        private void Map_Update(object sender, EventArgs e)
        {
            var map = sender as Map;
            if (map == null)
                return;

            var wildPokemons = map.Cells.SelectMany(c => c.WildPokemons).Select(p => new SpawnedPokemon(p, Configuration.Name));
            foreach (var pokemon in wildPokemons)
                PokemonHandler.HandleSpawnedPokemon(pokemon);

            var catchablePokemons = map.Cells.SelectMany(c => c.CatchablePokemons).Select(p => new SpawnedPokemon(p, Configuration.Name));
            foreach (var pokemon in catchablePokemons)
                PokemonHandler.HandleSpawnedPokemon(pokemon);

            if (Configuration.ProcessNearbyPokemon)
            {
                PokemonHandler.CleanNearbyPokemonList();

                var nearbyPokemons = map.Cells.SelectMany(c => c.NearbyPokemons).Select(p => new NearbyPokemon(p, Configuration.Name, Session.Player.Latitude, Session.Player.Longitude)).ToList();
                foreach (var pokemon in nearbyPokemons)
                    PokemonHandler.HandleNearbyPokemon(pokemon);
            }

            if (Configuration.ProcessGyms)
            {
                var gyms = map.Cells.SelectMany(x => x.Forts).Where(x => x.Type == FortType.Gym).ToList();
                foreach (var gym in gyms)
                    GymHandler.Handle(gym);
            }

            var nextPosition = Walker.GetNextPosition();
            Session.Player.SetCoordinates(nextPosition.Latitude, nextPosition.Longitude);
        }
    }
}
