using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FlightDataGenerator
{
    class Program
    {
        private static int FlightsToGenerate = 10000;
        private static int RadiusOfEarthInKm = 6373;
        private static Random r;

        private static IList<AirportData> AirportList;
        private static IList<AirlineData> AirlineList;
        private static IList<RouteData> RouteList;

        static void Main(string[] args)
        {
            r = new Random();
            // Load Airport Data.
            AirportList = LoadAirportData("airports.dat");
            // Load Airline Data.
            AirlineList = LoadAirlineData("airlines.dat");
            // Load Route Data.
            RouteList = LoadRouteData("routes.dat");

            var generatedFlights = GenerateFlightData();
            WriteFlightsToFile(generatedFlights);

            Console.WriteLine("Press Enter/Return to exit...");
            Console.ReadLine();
        }

        public static IList<AirportData> LoadAirportData(string fileName)
        {
            var airports = new List<AirportData>();
            int errorCount = 0;
            using (var airportDataStream = OpenFile(fileName))
            {
                string line = "";
                while((line = airportDataStream.ReadLine()) != null)
                {
                    var dataArray = Regex.Split(line, ",(?!\\s)");
                    if (dataArray.Length != 12)
                    {
                        errorCount++;
                    } else
                    {
                        var currentAirport = new AirportData()
                        {
                            AirportID = Int32.Parse(dataArray[0]),
                            Name = dataArray[1].Replace("\"", ""),
                            City = dataArray[2].Replace("\"", ""),
                            Country = dataArray[3].Replace("\"", ""),
                            IATAFAA = dataArray[4].Replace("\"", ""),
                            ICAO = dataArray[5].Replace("\"", ""),
                            Latitude = Double.Parse(dataArray[6]),
                            Longitude = Double.Parse(dataArray[7]),
                            Altitude = Double.Parse(dataArray[8]),
                            Timezone = Double.Parse(dataArray[9]),
                            DST = dataArray[10].Replace("\"", "")
                        };
                        airports.Add(currentAirport);
                    }
                }
            }
            Console.WriteLine("Error Count loading Airports: " + errorCount);
            return airports;            
        }

        public static IList<AirlineData> LoadAirlineData(string fileName)
        {
            var airlines = new List<AirlineData>();
            int errorCount = 0;
            using (var airlineDataStream = OpenFile(fileName))
            {
                string line = "";
                while ((line = airlineDataStream.ReadLine()) != null)
                {
                    var dataArray = Regex.Split(line, ",(?!\\s)");
                    if (dataArray.Length != 8)
                    {
                        errorCount++;
                    }
                    else
                    {
                        var currentAirline = new AirlineData()
                        {
                            AirlineID = Int32.Parse(dataArray[0]),
                            Name = dataArray[1].Replace("\"", ""),
                            Alias = dataArray[2].Replace("\"", ""),
                            IATA = dataArray[3].Replace("\"", ""),
                            ICAO = dataArray[4].Replace("\"", ""),
                            Callsign = dataArray[5].Replace("\"", ""),
                            Country = dataArray[6].Replace("\"", ""),
                        };
                        airlines.Add(currentAirline);
                    }
                }
            }
            Console.WriteLine("Error Count loading Airlines: " + errorCount);
            return airlines;
        }

        public static IList<RouteData> LoadRouteData(string fileName)
        {
            var routes = new List<RouteData>();
            int errorCount = 0;
            using (var routeDataStream = OpenFile(fileName))
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
                        if (!dataArray[3].Equals("\\N") && !dataArray[5].Equals("\\N"))
                        {
                            var airportIds = AirportList.Select(s => s.AirportID);
                            
                            if (airportIds.Contains(Int32.Parse(dataArray[3])) && 
                                airportIds.Contains(Int32.Parse(dataArray[5]))) {
                                var currentRoute = new RouteData()
                                {
                                    Airline = dataArray[0].Replace("\"", ""),
                                    AirlineID = Int32.Parse(dataArray[1].Equals("\\N") ? "0" : dataArray[1]),
                                    SourceAirport = dataArray[2].Replace("\"", ""),
                                    SourceAirportId = Int32.Parse(dataArray[3]),
                                    DestinationAirport = dataArray[4].Replace("\"", ""),
                                    DestinationAirportId = Int32.Parse(dataArray[5]),
                                    Codeshare = dataArray[6].Replace("\"", ""),
                                    Stops = dataArray[7].Replace("\"", ""),
                                    Equipment = dataArray[8].Replace("\"", "")
                                };
                                routes.Add(currentRoute);
                            } else
                            {
                                errorCount++;
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Error Count loading Routes: " + errorCount);
            return routes;
        }

        private static StreamReader OpenFile(string fileName)
        {
            var inputFile = File.OpenText(fileName);
            return inputFile;
        }

        public static IList<ConsumerFlight> GenerateFlightData()
        {
            var generatedFlights = new List<ConsumerFlight>();
            for(int i=0; i < FlightsToGenerate; i++)
            {
                var flight = new ConsumerFlight();
                var randomRoute = RouteList.ElementAt(r.Next(0, RouteList.Count));
//                Console.WriteLine("Random number: " + r.Next(0, RouteList.Count));
                generatedFlights.Add(GenerateFlightFromRoute(randomRoute, i));
            }
            return generatedFlights;
        }

        public static ConsumerFlight GenerateFlightFromRoute(RouteData route, int flightId)
        {
            var sourceAirport = AirportList.Where(s => s.AirportID == route.SourceAirportId).First();
            var destinationAirport = AirportList.Where(d => d.AirportID == route.DestinationAirportId).First();
            var distance = CalculateDistanceBetweenTwoAirports(sourceAirport, destinationAirport);
            var price = GeneratePriceBasedOnDistance(distance);
            var flight = new ConsumerFlight()
            {
                FlightId = flightId,
                SourceAirportId = sourceAirport.AirportID,
                SourceLongitude = sourceAirport.Longitude,
                SourceLatitude = sourceAirport.Latitude,
                DestinationAirportId = destinationAirport.AirportID,
                DestinationLongitude = destinationAirport.Longitude,
                DestinationLatitude = destinationAirport.Latitude,
                PointToPointDistance = distance,
                Price = price
            };
            return flight;
        }

        private static double CalculateDistanceBetweenTwoAirports(AirportData source, AirportData destination)
        {
            var dlon = destination.Longitude - source.Longitude;
            var dlat = destination.Latitude - source.Latitude;
            var a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(source.Latitude) * Math.Cos(destination.Latitude) * Math.Pow(Math.Sin(dlon / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = RadiusOfEarthInKm * c;
            return d;
        }

        private static double GeneratePriceBasedOnDistance(double distance)
        {
            double price = 0;
            var pricePerKm = r.NextDouble()*0.36953;
            price = distance * pricePerKm;
            return Math.Round(price, 2);
        }

        private static void WriteFlightsToFile(IList<ConsumerFlight> flightList)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var flight in flightList)
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
                sb.Append("\n");
            }
//            Console.Write(sb.ToString());
            File.AppendAllText("flights.dat", sb.ToString());
            Console.WriteLine("Wrote " + flightList.Count() + " to the file 'flights.dat'");
        }
    }
}
