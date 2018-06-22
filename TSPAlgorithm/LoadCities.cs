using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPAlgorithm
{
    class City
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public string CityName { get; set; }

        public City(decimal x, decimal y, string cityName)
        {
            X = x;
            Y = y;
            CityName = cityName;
        }
    }
    class LoadCities
    {
        public List<City> ReadCitiesLocationFromFile(string fileName)
        {
            var lines = File.ReadAllLines($"{fileName}");
            List<City> listOfCities = new List<City>();
            int i = 0;
            foreach (var line in lines)
            {
                var data = line.Split(';');
                listOfCities.Add(new City(Convert.ToInt32(data[1]), Convert.ToInt32(data[2]), data[0]));
            }
            return listOfCities;
        }
    }
}
