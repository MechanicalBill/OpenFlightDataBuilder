using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDataGenerator
{
    public class AirlineData
    {
        public int AirlineID { get; set; }      //Unique OpenFlights identifier for this airline.
        public string Name { get; set; }    //Name of the airline.
        public string Alias { get; set; }   //Alias of the airline. For example, All Nippon Airways is commonly known as "ANA".
        public string IATA { get; set; }    //2-letter IATA code, if available.
        public string ICAO { get; set; }    //3-letter ICAO code, if available.
        public string Callsign { get; set; }        //Airline callsign.
        public string Country { get; set; } //Country or territory where airline is incorporated.
    }
}
