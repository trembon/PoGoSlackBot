using Google.Protobuf;
using NLog;
using POGOLib.Net;
using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PoGoSlackBot.Entities;
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
        private static readonly Logger log = LogManager.GetLogger("GymHandler");

        private PogoInstance pogoInstance;

        private Dictionary<string, CachedGymData> cachedGyms;

        public GymHandler(PogoInstance pogoInstance)
        {
            this.pogoInstance = pogoInstance;
            this.cachedGyms = new Dictionary<string, CachedGymData>();
        }

        public void Handle(FortData gym)
        {
            if (!cachedGyms.ContainsKey(gym.Id))
            {
                cachedGyms.Add(gym.Id, new CachedGymData { GymData = gym });
            }
            else
            {
                var cachedData = cachedGyms[gym.Id];

                if (cachedData.GymDetails == null)
                {
                    cachedData.GymDetails = GetGymDetails(gym);
                    if (cachedData.GymDetails == null)
                        return;
                }

                Messages.IMessage message = null;
                if (gym.IsInBattle && !cachedData.GymData.IsInBattle)
                {
                    message = new GymUnderAttackMessage(gym, cachedData.GymDetails, pogoInstance.Configuration);
                }
                else if (gym.OwnedByTeam == TeamColor.Neutral && cachedData.GymData.OwnedByTeam != TeamColor.Neutral)
                {
                    message = new GymNeutralMessage(gym, cachedData.GymDetails, pogoInstance.Configuration);
                }
                else if (gym.OwnedByTeam != cachedData.GymData.OwnedByTeam)
                {
                    message = new GymHasBeenTakenMessage(gym, cachedData.GymDetails, pogoInstance.Configuration);
                }

                if (message != null)
                    message.Send();

                cachedData.GymData = gym;
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

        private class CachedGymData
        {
            public FortData GymData { get; set; }

            public FortDetailsResponse GymDetails { get; set; }
        }
    }
}
