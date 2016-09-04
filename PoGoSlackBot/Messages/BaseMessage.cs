using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slack.Webhooks;
using NLog;
using PoGoSlackBot.Configuration;

namespace PoGoSlackBot.Messages
{
    public abstract class BaseMessage : IMessage
    {
        protected static readonly Logger log = LogManager.GetLogger("Message");

        protected InstanceConfiguration configuration;

        public BaseMessage(InstanceConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        protected virtual SlackMessage CreateMessage()
        {
            return new SlackMessage
            {
                Channel = configuration.SlackChannel,
                IconEmoji = Emoji.Ghost,
                Username = configuration.SlackBotName
            };
        }

        public virtual void Send()
        {
            var message = CreateMessage();
            if (message != null)
            {
                try
                {
                    var slackClient = new SlackClient(configuration.SlackWebHookURL);
                    slackClient.Post(message);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Unable to send message to slack");
                }
            }
        }
    }
}
