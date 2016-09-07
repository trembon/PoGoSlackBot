using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slack.Webhooks;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.Entities;

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
                Fallback = String.Format("{0} is nearby.", nearbyPokemon.PokemonID),

                AuthorName = String.Format("{0} is nearby.", nearbyPokemon.PokemonID)
            };

            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.ImageURLFormat))
                slackAttachment.AuthorIcon = String.Format(configuration.MainConfiguration.ImageURLFormat, nearbyPokemon.PokemonID.ToString().ToLower());

            message.Attachments = new List<SlackAttachment> { slackAttachment };

            return message;
        }
    }
}
