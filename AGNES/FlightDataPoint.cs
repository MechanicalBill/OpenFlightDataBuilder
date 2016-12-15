using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGNES
{
    class FlightDataPoint
    {
        public FlightDataPoint()
        {
            ClusteringLevel = 0;
        }
        public int FlightId { get; set; }
        public double PointToPointDistance { get; set; }
        public double Price { get; set; }
        public double ClusteringLevel { get; set; }

        public double EuclideanDistanceTo(FlightDataPoint p)
        {
            return Math.Sqrt(Math.Pow(p.PointToPointDistance - this.PointToPointDistance, 2) + Math.Pow(p.Price - this.Price, 2));
        }
    }
}
