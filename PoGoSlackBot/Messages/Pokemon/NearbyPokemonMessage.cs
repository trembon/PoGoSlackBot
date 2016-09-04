using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slack.Webhooks;
using POGOProtos.Map.Pokemon;
using PoGoSlackBot.Configuration;

namespace PoGoSlackBot.Messages.Pokemon
{
    public class NearbyPokemonMessage : BaseMessage
    {
        private NearbyPokemon nearbyPokemon;

        public NearbyPokemonMessage(NearbyPokemon nearbyPokemon, InstanceConfiguration configuration) : base(configuration)
        {
            this.nearbyPokemon = nearbyPokemon;
        }

        protected override SlackMessage CreateMessage()
        {
            var message =  base.CreateMessage();

            var slackAttachment = new SlackAttachment
            {
                Color = "#ef7d00",
                Fallback = String.Format("{0} is nearby.", nearbyPokemon.PokemonId),

                AuthorName = String.Format("{0} is nearby.", nearbyPokemon.PokemonId)
            };

            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.ImageURLFormat))
                slackAttachment.AuthorIcon = String.Format(configuration.MainConfiguration.ImageURLFormat, nearbyPokemon.PokemonId.ToString().ToLower());

            message.Attachments = new List<SlackAttachment> { slackAttachment };

            return message;
        }
    }
}
