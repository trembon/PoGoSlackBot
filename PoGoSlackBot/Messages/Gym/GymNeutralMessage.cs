using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PoGoSlackBot.Configuration;
using PoGoSlackBot.Extensions;
using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Messages.Gym
{
    public class GymNeutralMessage : BaseMessage
    {
        private FortData mapGymData;
        private FortDetailsResponse gymDetails;

        public GymNeutralMessage(FortData mapGymData, FortDetailsResponse gymDetails, InstanceConfiguration configuration) : base(configuration)
        {
            this.mapGymData = mapGymData;
            this.gymDetails = gymDetails;
        }

        protected override SlackMessage CreateMessage()
        {
            var message = base.CreateMessage();

            var slackAttachment = new SlackAttachment
            {
                Color = "#0000FF",
                Fallback = String.Format("Gym, {0}, is now neutral.", gymDetails.Name),

                AuthorName = String.Format("Gym, {0}, is now neutral.", gymDetails.Name),

                ThumbUrl = gymDetails.ImageUrls.FirstOrDefault(),
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
