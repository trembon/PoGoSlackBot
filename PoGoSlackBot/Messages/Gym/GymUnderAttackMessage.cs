using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slack.Webhooks;
using POGOProtos.Networking.Responses;
using PoGoSlackBot.Extensions;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.Entities;

namespace PoGoSlackBot.Messages.Gym
{
    public class GymUnderAttackMessage : BaseMessage
    {
        private GymDetails gymDetails;

        public GymUnderAttackMessage(GymDetails gymDetails, InstanceConfiguration configuration) : base(configuration)
        {
            this.gymDetails = gymDetails;
        }

        protected override SlackMessage CreateMessage()
        {
            var message = base.CreateMessage();

            var slackAttachment = new SlackAttachment
            {
                Color = "#0000FF",
                Fallback = String.Format("Gym, {0}, is under attack!", gymDetails.Name),

                AuthorName = String.Format("Gym, {0}, is under attack!", gymDetails.Name),

                ThumbUrl = gymDetails.ImageURL,
                
                Fields = new List<SlackField>
                {
                    new SlackField
                    {
                        Title = "Owner",
                        Value = gymDetails.Owner.ToTeamName(),
                        Short = true
                    },
                    new SlackField
                    {
                        Title = "Strongest Pokemon",
                        Value = gymDetails.StrongestPokemon.ToString(),
                        Short = true
                    }
                }
            };

            if (!String.IsNullOrWhiteSpace(configuration.MainConfiguration.MapURLFormat))
            {
                slackAttachment.Title = "View on map";
                slackAttachment.TitleLink = String.Format(configuration.MainConfiguration.MapURLFormat, gymDetails.Latitude.ToString().Replace(",", "."), gymDetails.Longitude.ToString().Replace(",", "."));
            }

            message.Attachments = new List<SlackAttachment> { slackAttachment };

            return message;
        }
    }
}
