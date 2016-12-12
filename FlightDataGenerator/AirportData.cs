using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightDataGenerator
{
    public class AirportData
    {
        public int AirportID { get; set; }       //Unique OpenFlights identifier for this airport.
        public string Name { get; set; }    //Name of airport.May or may not contain the City name.
        public string City { get; set; } //Main city served by airport. May be spelled differently from Name.
        public string Country { get; set; } //Country or territory where airport is located.
        public string IATAFAA { get; set; }    //3-letter FAA code, for airports located in Country "United States of America". 3-letter IATA code, for all other airports. Blank if not assigned.
        public string ICAO { get; set; }    //4-letter ICAO code. Blank if not assigned.
        public double Latitude { get; set; } //Decimal degrees, usually to six significant digits.Negative is South, positive is North.
        public double Longitude { get; set; } //Decimal degrees, usually to six significant digits.Negative is West, positive is East.
        public double Altitude { get; set; } //In feet.
        public double Timezone { get; set; } //Hours offset from UTC. Fractional hours are expressed as decimals, eg.India is 5.5.
        public string DST { get; set; } //Daylight savings time. One of E (Europe), A(US/Canada), S(South America), O(Australia), Z(New Zealand), N(None) or U(Unknown). 
    }
}
