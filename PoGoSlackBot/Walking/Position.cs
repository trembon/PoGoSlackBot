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
            return String.Format("Lat {0}, Long {1}", Latitude, Longitude);
        }
    }
}
