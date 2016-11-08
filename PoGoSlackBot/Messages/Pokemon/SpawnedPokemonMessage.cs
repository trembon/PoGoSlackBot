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

        public SpawnedPokemonMessage(SpawnedPokemon spawnedPokemon, InstanceConfiguration configuration) : base(configuration)
        {
            this.spawnedPokemon = spawnedPokemon;
        }

        protected override SlackMessage CreateMessage()
        {
            var message = base.CreateMessage();

            var slackAttachment = new SlackAttachment
            {
                Color = "#36a64f",
                Fallback = String.Format("Wild {0} has spawned!", spawnedPokemon.PokemonID),

                AuthorName = String.Format("Wild {0} has spawned!", spawnedPokemon.PokemonID),

                Fields = new List<SlackField>()
            };
            
            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.ImageURLFormat))
                slackAttachment.ThumbUrl = String.Format(configuration.MainConfiguration.ImageURLFormat, spawnedPokemon.PokemonID.ToString().ToLower());

            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.MapURLFormat))
            {
                slackAttachment.Title = "View on map";
                slackAttachment.TitleLink = String.Format(configuration.MainConfiguration.MapURLFormat, spawnedPokemon.Latitude.ToString().Replace(",", "."), spawnedPokemon.Longitude.ToString().Replace(",", "."));
            }

            if (configuration.HomePosition != null)
            {
                slackAttachment.Fields.Add(new SlackField
                {
                    Title = "Distance",
                    Value = $"{configuration.HomePosition.DistanceTo(spawnedPokemon.Latitude, spawnedPokemon.Longitude)}m",
                    Short = true
                });
            }

            // TODO: add logic to caclulate this from the database
            //if (true)
            //{
            //    slackAttachment.Fields.Add(new SlackField
            //    {
            //        Title = "Despawning",
            //        Value = spawnedPokemon.Despawn.ToString(),
            //        Short = true
            //    });
            //}

            message.Attachments = new List<SlackAttachment> { slackAttachment };

            return message;
        }
    }
}
