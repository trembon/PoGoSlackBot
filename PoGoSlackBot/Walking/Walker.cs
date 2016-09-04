using NLog;
using System;

namespace PoGoSlackBot.Walking
{
    public class Walker
    {
        private static readonly Logger log = LogManager.GetLogger("Walker");

        private int currentPosition;
        private PositionList positions;

        public Walker(PositionList positions)
        {
            this.currentPosition = -1;
            this.positions = positions;

            if (positions == null || positions.Count == 0)
                throw new ArgumentNullException("WalkingPoints");
        }

        public Position GetNextPosition()
        {
            bool starting = false;
            if (currentPosition == -1)
            {
                currentPosition = GetRandomPosition();
                starting = true;
            }

            currentPosition++;
            if (currentPosition >= positions.Count)
                currentPosition = 0;

            var position = positions[currentPosition];

            if (starting)
            {
                log.Info("Starting to walk at {0}", position);
            }
            else
            {
                if (positions.Count > 1)
                    log.Info("Walking to {0}", position);
            }

            return position;
        }

        private int GetRandomPosition()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(0, positions.Count);
        }
    }
}
