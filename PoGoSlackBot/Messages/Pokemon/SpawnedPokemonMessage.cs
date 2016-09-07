using POGOProtos.Enums;
using POGOProtos.Map.Pokemon;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.Entities;
using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Messages.Pokemon
{
    public class SpawnedPokemonMessage : BaseMessage
    {
        private SpawnedPokemon spawnedPokemon;
        private bool wasNearby;

        public SpawnedPokemonMessage(SpawnedPokemon spawnedPokemon, bool wasNearby, InstanceConfiguration configuration) : base(configuration)
        {
            this.spawnedPokemon = spawnedPokemon;
            this.wasNearby = wasNearby;
        }

        protected override SlackMessage CreateMessage()
        {
            var message = base.CreateMessage();

            var slackAttachment = new SlackAttachment
            {
                Color = "#36a64f",
                Fallback = String.Format("Wild {0} has spawned!", spawnedPokemon.PokemonID),

                AuthorName = String.Format("Wild {0} has spawned!", spawnedPokemon.PokemonID),

                Fields = new List<SlackField>
                {
                    new SlackField
                    {
                        Title = "Despawning",
                        Value = spawnedPokemon.Despawn.ToString(),
                        Short = true
                    }
                }
            };
            
            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.ImageURLFormat))
                slackAttachment.ThumbUrl = String.Format(configuration.MainConfiguration.ImageURLFormat, spawnedPokemon.PokemonID.ToString().ToLower());

            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.MapURLFormat))
            {
                slackAttachment.Title = "View on map";
                slackAttachment.TitleLink = String.Format(configuration.MainConfiguration.MapURLFormat, spawnedPokemon.Latitude.ToString().Replace(",", "."), spawnedPokemon.Longitude.ToString().Replace(",", "."));
            }

            if (configuration.ProcessNearbyPokemon)
            {
                slackAttachment.Fields.Add(new SlackField
                {
                    Title = "Was nearby",
                    Value = wasNearby ? "Yes" : "No",
                    Short = true
                });
            }

            message.Attachments = new List<SlackAttachment> { slackAttachment };

            return message;
        }
    }
}
