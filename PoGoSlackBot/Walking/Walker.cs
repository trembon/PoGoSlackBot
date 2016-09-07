using GeoCoordinatePortable;
using NLog;
using System;

namespace PoGoSlackBot.Walking
{
    public class Walker
    {
        private readonly Logger log;

        private int currentIndex;
        private Position currentPosition;
        private PositionList positions;

        public Walker(string instanceName, PositionList positions)
        {
            this.log = LogManager.GetLogger($"Walker ({instanceName})");

            this.currentIndex = -1;
            this.positions = positions;

            if (positions == null || positions.Count == 0)
                throw new ArgumentNullException("WalkingPoints");
        }

        public Position GetNextPosition()
        {
            bool starting = false;
            if (currentIndex == -1)
            {
                currentIndex = GetRandomPosition();
                starting = true;
            }
            
            currentIndex++;
            if (currentIndex >= positions.Count)
                currentIndex = 0;

            var oldPosition = currentPosition;
            currentPosition = positions[currentIndex];

            if (starting)
            {
                log.Info("Starting to walk at {0}", currentPosition);
            }
            else
            {
                if (positions.Count > 1)
                    log.Info("Walking to {0} (Distance: {1}m)", currentPosition, oldPosition.DistanceTo(currentPosition));
            }

            return currentPosition;
        }

        private int GetRandomPosition()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(0, positions.Count);
        }
    }
}
