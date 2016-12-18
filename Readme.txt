Open the solution "OpenFlightsDataBuilder.sln" in Visual Studio 2015. This solution contains both the data builder project as well as the AGNES algorithm project.

To Run the AGNES algorithm:

Initially, it is set up to run the AGNES algorithm using a csv file named "flights.dat". Place the flights.dat file in the AGNES\bin\debug folder. You should now be able to just run the program in Visual Studio to run the algorithm.

Alternately, after compiling the program in visual studio, the program is designed to accept one command line parameter (the file name to process), and it can be run as such:
> AGNES flights.dat


To Generate new Flight data:

Right click the topmost item in the Solution Explorer in Visual Studio and click Properties. Change the "Single startup project" from AGNES to FlightDataGenerator. Now, place the attached 'airlines.dat', 'airports.dat' and 'routes.dat' into the FlightDataGenerator\bin\debug folder. You can change the global "FlightsToGenerate" variable to change the number of flights that will be created out of the base data. Now you can simply run the program in Visual Studio, and a 'flights.dat' file will be created and/or appended to. This file can be processed by all 3 algorithms.
