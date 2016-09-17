using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.Walking
{
    public class Position
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"Lat {Latitude}, Long {Longitude}";
        }

        public int DistanceTo(Position otherPosition)
        {
            GeoCoordinate otherPositionCoordinate = new GeoCoordinate(otherPosition.Latitude, otherPosition.Longitude);
            GeoCoordinate currentPositionCoordinate = new GeoCoordinate(this.Latitude, this.Longitude);

            return (int)currentPositionCoordinate.GetDistanceTo(otherPositionCoordinate);
        }

        public int DistanceTo(double latitude, double longitude)
        {
            return this.DistanceTo(new Position { Latitude = latitude, Longitude = longitude });
        }
    }
}
