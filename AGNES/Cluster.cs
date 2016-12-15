using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGNES
{
    class Cluster
    {
        public Cluster()
        {
            Points = new List<FlightDataPoint>();
        }

        public int ClusterId { get; set; }
        public IList<FlightDataPoint> Points { get; set; }

        public void MergeInto(Cluster destination)
        {
            foreach(var point in this.Points)
            {
                destination.Points.Add(point);
            }
            this.Points.Clear();
        }

        public double SingleLinkDistanceTo(Cluster other)
        {
            var minDistance = -1.0;
            foreach(var point in this.Points)
            {
                foreach(var otherPoint in other.Points)
                {
                    var distance = point.EuclideanDistanceTo(otherPoint);
                    if (distance < minDistance || minDistance < 0)
                    {
                        minDistance = distance;
                    }
                }
            }
            return minDistance;
        }
    }
}
