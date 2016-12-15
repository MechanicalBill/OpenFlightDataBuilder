using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using FlightDataGenerator;

namespace AGNES
{
    class Program
    {
        private static IList<FlightDataPoint> DataPoints;
        private static IList<ConsumerFlight> Flights;

        static void Main(string[] args)
        {
            Flights = LoadFlightData(args[0]);

            DataPoints = ConvertFlightsToDataPoints(Flights);
            RunAGNESAlgorithm();

            Console.WriteLine("Press Enter/Return to exit...");
            Console.ReadLine();
        }

        public static IList<ConsumerFlight> LoadFlightData(string filename)
        {
            var errorCount = 0;

            var Flights = new List<ConsumerFlight>();
            
            using (var routeDataStream = File.OpenText(filename))
            {
                string line = "";
                while ((line = routeDataStream.ReadLine()) != null)
                {
                    var dataArray = Regex.Split(line, ",(?!\\s)");
                    if (dataArray.Length != 9)
                    {
                        errorCount++;
                    }
                    else
                    {
                        var currentFlight = new ConsumerFlight()
                        {
                            FlightId = Int32.Parse(dataArray[0]),
                            SourceAirportId = Int32.Parse(dataArray[1]),
                            SourceLongitude = Double.Parse(dataArray[2]),
                            SourceLatitude = Double.Parse(dataArray[3]),
                            DestinationAirportId = Int32.Parse(dataArray[4]),
                            DestinationLongitude = Double.Parse(dataArray[5]),
                            DestinationLatitude = Double.Parse(dataArray[6]),
                            PointToPointDistance = Double.Parse(dataArray[7]),
                            Price = Double.Parse(dataArray[8])
                        };
                        Flights.Add(currentFlight);
                    }
                }
            }

            return Flights;
        }

        public static IList<FlightDataPoint> ConvertFlightsToDataPoints(IList<ConsumerFlight> flights)
        {
            var dataPoints = new List<FlightDataPoint>();

            foreach (var flight in flights)
            {
                var newPoint = new FlightDataPoint()
                {
                    FlightId = flight.FlightId,
                    PointToPointDistance = flight.PointToPointDistance,
                    Price = flight.Price
                };
                dataPoints.Add(newPoint);
            }
            return dataPoints;
        }

        public static void RunAGNESAlgorithm()
        {
            var clusters = BuildAtomicClusters(DataPoints);
            var clusteringOrder = 1;
            while (clusters.Count() > 1)
            {
                var distanceMatrix = ComputeDistanceMatrix(clusters);
                clusters = MergeClustersBasedOnDistance(clusters, distanceMatrix, clusteringOrder);
                clusteringOrder++;
            }
            WriteClusteringDiagramToFile(clusters[0]); //Omitted for time reasons.
            WriteClusteredFlightsToFile(clusters[0]);
        }

        private static IList<Cluster> MergeClustersBasedOnDistance(IList<Cluster> clusters, double[,] distanceMatrix, int clusteringOrder)
        {
            double minDistance = 0;
            int copyFromClusterIndex = 0, copyToClusterIndex = 0;
            for(int i = 0; i < clusters.Count(); i++)
            {
                for(int j=i+1; j < clusters.Count(); j++)
                {
                    if(minDistance == 0 || minDistance > distanceMatrix[i,j])
                    {
                        minDistance = distanceMatrix[i, j];
                        copyFromClusterIndex = j;
                        copyToClusterIndex = i;
                    }
                }
            }
            foreach (var point in clusters[copyToClusterIndex].Points)
            {
                if(point.ClusteringLevel == 0)
                {
                    point.ClusteringLevel = minDistance;
                }
            }
            foreach (var point in clusters[copyFromClusterIndex].Points)
            {
                if (point.ClusteringLevel == 0)
                {
                    point.ClusteringLevel = minDistance;
                }
            }
            clusters[copyFromClusterIndex].MergeInto(clusters[copyToClusterIndex]);
            clusters.RemoveAt(copyFromClusterIndex);
            return clusters;
        }

        private static IList<Cluster> BuildAtomicClusters(IList<FlightDataPoint> dataPoints)
        {
            var atomicClusters = new List<Cluster>();
            foreach(var point in dataPoints)
            {
                var newCluster = new Cluster()
                {
                    ClusterId = point.FlightId
                };
                newCluster.Points.Add(point);
                atomicClusters.Add(newCluster);
            }
            return atomicClusters;
        }

        public static double[,] ComputeDistanceMatrix(IList<Cluster> clusters)
        {
            var distanceMatrix = new double[clusters.Count(), clusters.Count()];

            for (int i = 0; i < clusters.Count(); i++)
            {
                for (int j = 0; j < clusters.Count(); j++)
                {
                    distanceMatrix[i, j] = clusters[i].SingleLinkDistanceTo(clusters[j]);
                    Console.Write(" " + Math.Round(distanceMatrix[i, j], 2));
                }
                Console.Write("\n");
            }
            return distanceMatrix;
        }

        private static void WriteClusteringDiagramToFile(Cluster cluster)
        {
            Console.Write("\n\n");
            StringBuilder sb = new StringBuilder();
            sb.Append("LEVEL,,");
            foreach (var point in cluster.Points)
            {
                sb.Append(point.FlightId);
                sb.Append(",,");
            }
            sb.Append("\n");

            var orderedLevels = new List<double>();
            foreach(var point in cluster.Points)
            {
                if (!orderedLevels.Contains(point.ClusteringLevel))
                {
                    orderedLevels.Add(point.ClusteringLevel);
                }
            }
            orderedLevels.Sort();

            var tableData = new char[cluster.Points.Count(), cluster.Points.Count()];
            for(int i=0; i < orderedLevels.Count(); i++)
            {
                for(int j=0; j < cluster.Points.Count(); j++)
                {
                    var c = '-';
                    if(i > 0)
                    {
                        if(tableData[i-1,j].Equals('X') || cluster.Points[j].ClusteringLevel == orderedLevels[i])
                        {
                            c = 'X';
                        }
                    } else
                    {
                        if(cluster.Points[j].ClusteringLevel == orderedLevels[i])
                        {
                            c = 'X';
                        }
                    }
                    tableData[i, j] = c;
                }
            }

            for(int k=0; k < orderedLevels.Count(); k++)
            {
                sb.Append(orderedLevels[k] + ",,");
                for (int m = 0; m < tableData.GetLength(0); m++) {
                    sb.Append(tableData[k,m]);
                    if (tableData[k, m].Equals('X') && m + 1 < tableData.GetLength(0))
                    {
                        if (tableData[k, m + 1].Equals('X'))
                        {
                            sb.Append(",X,");
                        } else
                        {
                            sb.Append(",,");
                        }
                    }
                    else
                    {
                        sb.Append(",,");
                    }
                }
                sb.Append("\n");
            }

            File.AppendAllText("ClusteringDiagram.csv", sb.ToString());
        }

        private static void WriteClusteredFlightsToFile(Cluster cluster)
        {
            string filename = "ClusteredFlights.csv";

            StringBuilder sb = new StringBuilder();
            sb.Append("FlightID,SourceAirportId,SourceLongitude,SourceLatitude,DestinationAirportId,DestinationLongitude,DestinationLatitude,PointToPointDistance,Price,ClusteringLevel\n");
            foreach (var point in cluster.Points)
            {
                foreach (var flight in Flights)
                {                
                    if(flight.FlightId == point.FlightId)
                    {
                        sb.Append(flight.FlightId);
                        sb.Append(",");
                        sb.Append(flight.SourceAirportId.ToString());
                        sb.Append(",");
                        sb.Append(flight.SourceLongitude);
                        sb.Append(",");
                        sb.Append(flight.SourceLatitude);
                        sb.Append(",");
                        sb.Append(flight.DestinationAirportId.ToString());
                        sb.Append(",");
                        sb.Append(flight.DestinationLongitude);
                        sb.Append(",");
                        sb.Append(flight.DestinationLatitude);
                        sb.Append(",");
                        sb.Append(flight.PointToPointDistance);
                        sb.Append(",");
                        sb.Append(flight.Price);
                        sb.Append(",");
                        sb.Append(point.ClusteringLevel);
                        sb.Append("\n");
                    }
                }
            }
            File.AppendAllText(filename, sb.ToString());

            Console.WriteLine("Wrote flight records and clustering data to '" + filename + "'");
        }
    }
}
