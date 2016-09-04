using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Messages
{
    public interface IMessage
    {
        void Send();
    }
}
