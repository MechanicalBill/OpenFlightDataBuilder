using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDataGenerator
{
    class RouteData
    {
        public string Airline { get; set; } //2-letter (IATA) or 3-letter (ICAO) code of the airline.
        public int AirlineID { get; set; } //Unique OpenFlights identifier for airline (see Airline).
        public string SourceAirport	{ get; set; } //3-letter (IATA) or 4-letter (ICAO) code of the source airport.
        public int SourceAirportId { get; set; } //Unique OpenFlights identifier for source airport (see Airport)
        public string DestinationAirport { get; set; } //3-letter (IATA) or 4-letter (ICAO) code of the destination airport.
        public int DestinationAirportId { get; set; } //Unique OpenFlights identifier for destination airport (see Airport)
        public string Codeshare { get; set; } //"Y" if this flight is a codeshare (that is, not operated by Airline, but another carrier), empty otherwise.
        public string Stops { get; set; } //Number of stops on this flight ("0" for direct)
        public string Equipment { get; set; } //3-letter codes for plane type(s) generally used on this flight, separated by spaces
        public double Price { get; set; }
    }
}
