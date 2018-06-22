using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPAlgorithm
{
    class Population
    { 
        public List<Road> PopulationList { get; set; }
    }
    class Road
    {
        public List<City> CitiesList { get; set; }
        public decimal TotalDistance { get; set; }
    }
    class CalculateFunction
    {
            public decimal CalculateDistances(City A, City B)
            {
                decimal distanceX = (decimal)Math.Pow((double)A.X - (double)B.X, 2);
                decimal distanceY = (decimal)Math.Pow((double)A.Y - (double)B.Y, 2);
                decimal result = (decimal)Math.Sqrt((double)distanceX + (double)distanceY);
                return result;
            }
            public void CalculateTotalDistance(Road track, bool cykl)
            {
                for (int i = 0; i < track.CitiesList.Count; i++)
                {
                    int x = 0;
                    if (i == track.CitiesList.Count - 1)
                    {
                        if (cykl != true)
                        {
                            x = i;
                        }
                        if (cykl == true)
                        {
                            x = 0;
                        }
                    }
                    else
                    {
                        x = i + 1;
                    }

                    City cityA = track.CitiesList[i];
                    City cityB = track.CitiesList[x];
                  
                    var res = CalculateDistances(cityA, cityB);
                    track.TotalDistance += res;
                }
            }

            public void CalculatePopulationDistances(Population population, bool cykl)
            {
                var popSize = population.PopulationList.Count;
                for (int i = 0; i < popSize; i++)
                {
                    CalculateTotalDistance(population.PopulationList[i], cykl);
                }
            }
            public void OrderPopulation(Population population)
            {
                population.PopulationList.Sort(delegate (Road trackA, Road trackB)
                {
                    return trackA.TotalDistance.CompareTo(trackB.TotalDistance);
                });
            }
        }
}
