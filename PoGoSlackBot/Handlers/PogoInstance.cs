using Google.Protobuf;
using NLog;
using POGOLib.Net;
using POGOLib.Pokemon;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Requests;
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
        public Session Session { get; set; }

        public PogoDB Database { get; }

        public InstanceConfiguration Configuration { get; }

        public Walker Walker { get; }

        public PokemonHandler PokemonHandler { get; }

        public GymHandler GymHandler { get; }

        public PogoInstance(InstanceConfiguration configuration)
        {
            this.Configuration = configuration;

            this.Database = new PogoDB(this.Configuration.Name);

            this.Walker = new Walker(this.Configuration.Name, this.Configuration.WalkingPoints);
            this.GymHandler = new GymHandler(this);
            this.PokemonHandler = new PokemonHandler(this);
        }
    }
}
