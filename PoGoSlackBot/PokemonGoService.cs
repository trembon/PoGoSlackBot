using POGOLib.Net.Authentication;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.Handlers;
using PoGoSlackBot.Walking;
using System.Collections.Generic;

namespace PoGoSlackBot
{
    public class PokemonGoService
    {
        private List<PogoInstance> pogoInstances;

        public PokemonGoService()
        {
            var config = MainConfiguration.Load();

            pogoInstances = new List<PogoInstance>(config.Instances.Count);

            foreach(var instance in config.Instances)
            {
                instance.MainConfiguration = config;

                var walker = new Walker(instance.Name, instance.WalkingPoints);
                var startPosition = walker.GetNextPosition();

                Login.GetSession(instance.GetLoginProvider(), startPosition.Latitude, startPosition.Longitude).ContinueWith(x =>
                {
                    var pogoInstance = new PogoInstance(instance, x.Result, walker);
                    pogoInstance.Session.StartupAsync().ContinueWith(y =>
                    {
                        pogoInstances.Add(pogoInstance);
                    });
                });
            }
        }

        public void Start()
        {
            //pogoInstances.ForEach(i => i.Session.StartupAsync().Start());
        }

        public void Stop()
        {
            pogoInstances.ForEach(i => i.Session.Shutdown());
        }
    }
}
