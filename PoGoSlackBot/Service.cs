using NLog;
using POGOLib.Net.Authentication;
using POGOLib.Pokemon;
using POGOProtos.Map.Fort;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.Entities;
using PoGoSlackBot.Handlers;
using PoGoSlackBot.Walking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace PoGoSlackBot
{
    public class Service : ServiceBase
    {
        public const string SERVICE_NAME = "Pokemon GO - Slack Bot";
        public const string SERVICE_DESC = "A Pokemon GO Slack bot that will scan for Pokemon for your slack channel";
        
        private static readonly Logger log = LogManager.GetLogger("Service");

        private List<PogoInstance> instances;

        public Service()
        {
            ServiceName = SERVICE_NAME;

            POGOLib.Logging.Logger.RegisterLogOutput((level, msg) =>
            {
                log.Info($"POGOLib, {level} - {msg}");
            });

            instances = new List<PogoInstance>();
            var mainConfiguration = MainConfiguration.Load();

            foreach (var configuration in mainConfiguration.Instances)
            {
                configuration.MainConfiguration = mainConfiguration;
                instances.Add(new PogoInstance(configuration));
            }
        }

        #region Service methods
        /// <summary>
        /// Starts the service with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Start(string[] args)
        {
            this.OnStart(args);
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected async override void OnStart(string[] args)
        {
            foreach (var instance in instances)
            {
                if (instance.Session != null)
                {
                    await instance.Session.StartupAsync();
                }
                else
                {
                    var startPosition = instance.Walker.GetNextPosition();
                    instance.Session = await Login.GetSession(instance.Configuration.GetLoginProvider(), startPosition.Latitude, startPosition.Longitude);

                    instance.Session.Map.Update += (sender, e) =>
                    {
                        var map = sender as Map;
                        if (map != null)
                            this.Map_Update(map, instance);
                    };

                    await instance.Session.StartupAsync();
                }
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            foreach(var instance in instances)
                if (instance.Session != null)
                    instance.Session.Shutdown();
        }
        #endregion
        
        private async void Map_Update(Map map, PogoInstance instance)
        {
            var wildPokemons = map.Cells.SelectMany(c => c.WildPokemons).Select(p => new SpawnedPokemon(p, instance.Configuration.Name));
            foreach (var pokemon in wildPokemons)
                instance.PokemonHandler.HandleSpawnedPokemon(pokemon);

            var catchablePokemons = map.Cells.SelectMany(c => c.CatchablePokemons).Select(p => new SpawnedPokemon(p, instance.Configuration.Name));
            foreach (var pokemon in catchablePokemons)
                instance.PokemonHandler.HandleSpawnedPokemon(pokemon);

            if (instance.Configuration.ProcessNearbyPokemon)
            {
                instance.PokemonHandler.CleanNearbyPokemonList();

                var nearbyPokemons = map.Cells.SelectMany(c => c.NearbyPokemons).Select(p => new NearbyPokemon(p, instance.Configuration.Name, instance.Session.Player.Latitude, instance.Session.Player.Longitude)).ToList();
                foreach (var pokemon in nearbyPokemons)
                    instance.PokemonHandler.HandleNearbyPokemon(pokemon);
            }

            if (instance.Configuration.ProcessGyms)
            {
                var gyms = map.Cells.SelectMany(x => x.Forts).Where(x => x.Type == FortType.Gym).ToList();
                foreach (var gym in gyms)
                    await instance.GymHandler.Handle(gym);
            }

            var nextPosition = instance.Walker.GetNextPosition();
            instance.Session.Player.SetCoordinates(nextPosition.Latitude, nextPosition.Longitude);
        }
    }
}
