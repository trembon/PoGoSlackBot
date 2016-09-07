using Google.Protobuf;
using NLog;
using POGOLib.Net;
using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PoGoSlackBot.Entities;
using PoGoSlackBot.Extensions;
using PoGoSlackBot.Messages.Gym;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Handlers
{
    public class GymHandler
    {
        private readonly Logger log;

        private PogoInstance pogoInstance;

        private Dictionary<string, GymDetails> gymStatus;

        public GymHandler(PogoInstance pogoInstance)
        {
            this.pogoInstance = pogoInstance;
            this.log = LogManager.GetLogger($"GymHandler ({pogoInstance.Configuration.Name})");

            this.gymStatus = pogoInstance.Database.GetLatestGymDetails();
        }

        public void Handle(FortData gym)
        {
            if (!gymStatus.ContainsKey(gym.Id))
            {
                var gymDetailsResponse = GetGymDetails(gym);
                if(gymDetailsResponse != null)
                {
                    var gymDetails = new GymDetails(gym, gymDetailsResponse.Name, gymDetailsResponse.ImageUrls.FirstOrDefault(), pogoInstance.Configuration.Name);

                    pogoInstance.Database.AddGymDetails(gymDetails);
                    gymStatus.Add(gym.Id, gymDetails);
                }
            }
            else
            {
                var gymDetails = gymStatus[gym.Id];

                bool isInBattle = gym.IsInBattle && !gymDetails.IsInBattle;
                bool isNowNeutral = gym.OwnedByTeam == TeamColor.Neutral && gymDetails.Owner != TeamColor.Neutral;
                bool hasChangedOwner = gym.OwnedByTeam != gymDetails.Owner;

                gymDetails.Update(gym);
                pogoInstance.Database.UpdateGymDetails(gymDetails);

                Messages.IMessage message = null;
                if (isInBattle)
                {
                    log.Info($"Gym, {gymDetails.Name}, is under attack");
                    message = new GymUnderAttackMessage(gymDetails, pogoInstance.Configuration);
                }
                else if (isNowNeutral)
                {
                    log.Info($"Gym, {gymDetails.Name}, is now neutral");
                    message = new GymNeutralMessage(gymDetails, pogoInstance.Configuration);
                }
                else if (hasChangedOwner)
                {
                    log.Info($"Gym, {gymDetails.Name}, has been taken by {gymDetails.Owner.ToTeamName()}");
                    message = new GymHasBeenTakenMessage(gymDetails, pogoInstance.Configuration);
                }

                if (message != null)
                    message.Send();
            }
        }

        private FortDetailsResponse GetGymDetails(FortData gymData)
        {
            try
            {
                var fortDetailsBytes = pogoInstance.Session.RpcClient.SendRemoteProcedureCall(new Request
                {
                    RequestType = RequestType.FortDetails,
                    RequestMessage = new FortDetailsMessage
                    {
                        FortId = gymData.Id,
                        Latitude = gymData.Latitude,
                        Longitude = gymData.Longitude
                    }.ToByteString()
                });

                return FortDetailsResponse.Parser.ParseFrom(fortDetailsBytes);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Unable to fetch gym details.");
                return null;
            }
        }
    }
}
