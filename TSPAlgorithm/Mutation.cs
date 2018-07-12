using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TSPAlgorithm
{
    public struct NeighborList
    {
        public string CityName;
        public int Neighbors;

        public NeighborList(string cityName, int neighbors)
        {
            CityName = cityName;
            Neighbors = neighbors;
        }
    }

    class Mutation
    {
        private readonly Random _rand = new Random();
        private CalculateFunction _calc = new CalculateFunction();
        public int Iterations { get; set; }
        public int RandomMutations = 0;
        public int RandomMutations2 = 0;
        public int mutatePopulationIterator = 0;
        bool cykl { get; set; }

        public Mutation(bool cykl)
        {
            this.cykl = cykl;
        }

        public void GenerateRandomPopulation(Population population, List<City> citiesList, int populationSize)
        {
            Road[] randomPopulation = new Road[populationSize];

            List<Road> randomPopulationList = new List<Road>();

            for (int i = 0; i < populationSize; i++)
            {
                randomPopulationList.Add(GenerateRandomPopulationMember(citiesList));
            }
            population.PopulationList = randomPopulationList;
        }
        private Road GenerateRandomPopulationMember(List<City> citiesList)
        {
            int[] citiesIndexTable = new int[citiesList.Count];

            for (int i = 0; i < citiesList.Count; i++)
            {
                citiesIndexTable[i] = i;
            }

            int[] randomIndexTable = new int[citiesList.Count];


            Road road = new Road();

            int size = citiesIndexTable.Length;
            List<City> newCityList = new List<City>();

            for (int i = 0; i < citiesList.Count; i++)
            {
                int randomIndex = _rand.Next(0, size);
                randomIndexTable[i] = citiesIndexTable[randomIndex];
                newCityList.Add(citiesList[randomIndexTable[i]]);
                for (int j = randomIndex; j < size - 1; j++)
                {
                    citiesIndexTable[j] = citiesIndexTable[j + 1];
                }
                size--;
            }

            bool test = randomIndexTable.Distinct().Count() == citiesIndexTable.Count();
            if (test == false)
            {
                Console.WriteLine("Błąd generatora populacji");
                Console.Read();
            }


            road.CitiesList = newCityList;
            return road;
        }

        private Road RandomMutationForCrossover(Road Parent)
        {
            Road child = new Road();
            City nullCity = new City(0, 0, "N");
            List<City> nullCityList = new List<City>();
            for (int j = 0; j < Parent.CitiesList.Count; j++)
            {
                nullCityList.Add(nullCity);
            }
            child.CitiesList = nullCityList;
            int rand1 = _rand.Next(0, Parent.CitiesList.Count);
            int rand2 = _rand.Next(0, Parent.CitiesList.Count);
            for (int j = 0; j < Parent.CitiesList.Count; j++)
            {
                if (j == rand1)
                {
                    child.CitiesList[j] = Parent.CitiesList[rand2];
                }
                else if (j == rand2)
                {
                    child.CitiesList[j] = Parent.CitiesList[rand1];
                }
                else
                {
                    child.CitiesList[j] = Parent.CitiesList[j];
                }
            }
            _calc.CalculateTotalDistance(child, cykl);
            return child;
        }

        public Population mutatePopulation(Road Parent, int populationSize)
        {
            List<City> citiesList = Parent.CitiesList;
            Population population = new Population();

            Road[] randomPopulation = new Road[populationSize];

            List<Road> randomPopulationList = new List<Road>();

            for (int i = 0; i < populationSize; i++)
            {
                randomPopulationList.Add(GenerateRandomPopulationMember(citiesList));
            }
            population.PopulationList = randomPopulationList;
            _calc.CalculatePopulationDistances(population, cykl);
            return population;
        }


        public void MutateOX1(Population population, List<City> listOfCities)
        {
            _calc.OrderPopulation(population);
            int sizeOfPopulation = population.PopulationList.Count;
            Road bestRoad = population.PopulationList[0];
            int licznikPoprawy = 0;
            int iterations = 0;
            do
            {
                decimal toKill1 = Convert.ToDecimal(sizeOfPopulation) * 0.5m;
                decimal toKill2 = Convert.ToDecimal(sizeOfPopulation) - toKill1;
                population.PopulationList.RemoveRange(Convert.ToInt32(toKill1), Convert.ToInt32(toKill2));
                if (licznikPoprawy > 15)
                {
                    Population newPopulation = mutatePopulation(population.PopulationList[0], Convert.ToInt32(population.PopulationList.Count/2));
                    for (int i = 0; i < newPopulation.PopulationList.Count ; i++)
                    {
                        population.PopulationList.Add(newPopulation.PopulationList[i]);
                        RandomMutations2++;
                    }
                }
                int z = 0;
                for (int i = population.PopulationList.Count; i < sizeOfPopulation; i++)
                {
                    int newSizeOfPopulation = population.PopulationList.Count;
                    int parent1Index = _rand.Next(0, newSizeOfPopulation);
                    int parent2Index = _rand.Next(0, newSizeOfPopulation);
                    Road parent1;
                    Road parent2;
                    parent1 = population.PopulationList[parent1Index];
                    parent2 = population.PopulationList[parent2Index];
                    int sizeOfRoad = population.PopulationList[0].CitiesList.Count;
                    Road child = new Road();
                    City nullCity = new City(0, 0, "N");
                    List<City> nullCityList = new List<City>();
                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        nullCityList.Add(nullCity);
                    }
                    child.CitiesList = nullCityList;
                    int cross1 = _rand.Next(0, sizeOfRoad - 1);
                    int cross2 = _rand.Next(cross1, sizeOfRoad);
                    int c2c1 = cross2 - cross1;
                    for (int j = cross1; j <= cross2; j++)
                    {
                        child.CitiesList[j] = parent1.CitiesList[j];
                    }
                    int childIndex = 0;
                    int parentIndex = 0;
                    do
                    {
                        if (child.CitiesList[childIndex].CityName != "N")
                        {
                            childIndex++;
                            continue;
                        }

                        if (!child.CitiesList.Contains(parent2.CitiesList[parentIndex]))
                        {
                            child.CitiesList[childIndex] = parent2.CitiesList[parentIndex];
                            childIndex++;
                            parentIndex++;
                        }
                        else
                        {
                            parentIndex++;
                        }
                    } while (childIndex < sizeOfRoad);
                    _calc.CalculateTotalDistance(child, cykl);
                    population.PopulationList.Add(child);
                    if (licznikPoprawy > 15)
                    {
                        Road child2 = RandomMutationForCrossover(child);
                        _calc.CalculateTotalDistance(child2, cykl);
                        population.PopulationList.Add(child2);
                    }
                }
                iterations++;

                _calc.OrderPopulation(population);
                decimal oldBest = bestRoad.TotalDistance;

                if (population.PopulationList[0].TotalDistance < bestRoad.TotalDistance)
                {
                    bestRoad = population.PopulationList[0];
                    licznikPoprawy = 0;
                }
                else
                {
                    licznikPoprawy++;
                }
                Iterations++;
                Console.WriteLine($"Iteracja {Iterations}, current best: {bestRoad.TotalDistance}");
                z++;
            } while (licznikPoprawy <= 31);
            Console.WriteLine(RandomMutations2);
        }

        private List<NeighborList> generateNeighborLists(List<NeighborList> NLL, string[] parent1, string[] parent2)
        {
            for (int i = 0; i < NLL.Count; i++)
            {
                NeighborList n1 = NLL[i];
                string city = n1.CityName;
                int lvl = GetNeighborLevel(parent1, city);
                lvl += GetNeighborLevel(parent2, city);
                n1.Neighbors = lvl;
                NLL[i] = n1;
            }
            return NLL;
        }

        private int GetNeighborLevel(string[] parent, string cityName)
        {
            int lvl = 0;
            for (int i = 0; i < parent.Length; i++)
            {
                if (parent[i] == cityName)
                {
                    if (i == 0)
                    {
                        if (parent[i + 1] != "X")
                        {
                            lvl++;
                        }
                    }
                    else if (i == parent.Length - 1)
                    {
                        if (parent[i - 1] != "X")
                        {
                            lvl++;
                        }
                    }
                    else
                    {
                        if (parent[i + 1] != "X")
                        {
                            lvl++;
                        }
                        if (parent[i - 1] != "X")
                        {
                            lvl++;
                        }
                    }
                }
            }

            return lvl;
        }

        public void MutateERO(Population population, List<City> citiesList)
        {
            _calc.OrderPopulation(population);
            int sizeOfPopulation = population.PopulationList.Count;
            int sizeOfRoad = population.PopulationList[0].CitiesList.Count;
            Road bestRoad = population.PopulationList[0];

            int licznikPoprawy = 0;

            List<NeighborList> NLList = new List<NeighborList>();

            for (int i = 0; i < sizeOfRoad; i++)
            {
                NLList.Add(new NeighborList(citiesList[i].CityName, 0));
            }


            do
            {
                decimal toKill1 = Convert.ToDecimal(sizeOfPopulation) * 0.2m;
                decimal toKill2 = Convert.ToDecimal(sizeOfPopulation) - toKill1;

                population.PopulationList.RemoveRange(Convert.ToInt16(toKill1), Convert.ToInt16(toKill2));

                int z = 0;
                for (int i = population.PopulationList.Count; i < sizeOfPopulation; i++)
                {
                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        NeighborList jList = new NeighborList();
                        jList.CityName = citiesList[j].CityName;
                        NLList[j] = jList;
                    }
                    int newSizeOfPopulation = population.PopulationList.Count;
                    int parent1Index = _rand.Next(0, newSizeOfPopulation);
                    int parent2Index = _rand.Next(0, newSizeOfPopulation);
                    Road parent1 = population.PopulationList[parent1Index];
                    Road parent2 = population.PopulationList[parent2Index];
                    Road child = new Road();
                    City nullCity = new City(0, 0, "N");
                    List<City> nullCityList = new List<City>();
                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        nullCityList.Add(nullCity);
                    }
                    child.CitiesList = nullCityList;

                    string[] parent1Table = new string[sizeOfRoad];
                    string[] parent2Table = new string[sizeOfRoad];
                    string[] childTable = new string[sizeOfRoad];


                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        parent1Table[j] = parent1.CitiesList[j].CityName;
                        parent2Table[j] = parent2.CitiesList[j].CityName;
                    }
                    NLList = generateNeighborLists(NLList, parent2Table, parent1Table);

                    childTable[0] = parent1Table[0];
                    parent1Table[0] = "X";

                    for (int j = 0; j < parent2Table.Length; j++)
                    {
                        if (parent2Table[j] == childTable[0])
                        {
                            parent2Table[j] = "X";
                        }
                    }


                    int idx = NLList.Select((value, index) => new { value, index })
                       .Where(x => x.value.CityName == childTable[0]).Select(x => x.index).FirstOrDefault();

                    NeighborList zaz = new NeighborList();
                    zaz.CityName = "X";
                    NLList[idx] = zaz;

                    for (int j = 1; j < sizeOfRoad; j++)
                    {
                        int r = 0;
                        List<string> minimalCities = new List<string>();
                        int minumum = 4;
                        for (int k = 0; k < sizeOfRoad; k++)
                        {
                            if (NLList[k].Neighbors < minumum && NLList[k].CityName != "X")
                            {
                                minumum = NLList[k].Neighbors;
                            }
                        }
                        for (int k = 0; k < sizeOfRoad; k++)
                        {
                            if (NLList[k].Neighbors == minumum)
                            {
                                minimalCities.Add(NLList[k].CityName);
                            }
                        }

                        if (minimalCities.Count == 0)
                        {
                            Console.WriteLine("AAA");
                        }

                        try
                        {
                            r = _rand.Next(0, minimalCities.Count);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ERROR: Empty cities list with minimal neighbours!!!");
                            throw;
                        }

                        childTable[j] = minimalCities[r];
                        for (int k = 0; k < parent2Table.Length; k++)
                        {
                            if (parent2Table[k] == minimalCities[r])
                            {
                                parent2Table[k] = "X";
                            }
                        }
                        for (int k = 0; k < parent1Table.Length; k++)
                        {
                            if (parent1Table[k] == minimalCities[r])
                            {
                                parent1Table[k] = "X";
                            }
                        }
                        int indeks = NLList.Select((value, index) => new { value, index })
                            .Where(x => x.value.CityName == childTable[j]).Select(x => x.index).FirstOrDefault();

                        NeighborList zaa = new NeighborList();
                        zaa.CityName = "X";
                        r = indeks;
                        NLList[indeks] = zaa;
                    }
                    City[] citiesTable = new City[sizeOfRoad];
                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        City c = new City(0, 0, "X");
                        var cc = citiesList.Where(x => x.CityName == childTable[j]).GroupBy(x => x.CityName)
                            .Select(x => x.FirstOrDefault());
                        foreach (var VARIABLE in cc)
                        {
                            c = VARIABLE;
                        }
                        citiesTable[j] = c;
                    }

                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        child.CitiesList[j] = citiesTable[j];
                    }
                    _calc.CalculateTotalDistance(child, cykl);
                    population.PopulationList.Add(child);
                }
                decimal oldBest = bestRoad.TotalDistance;
                decimal pretender = population.PopulationList[0].TotalDistance;

                if (population.PopulationList[0].TotalDistance < bestRoad.TotalDistance)
                {
                    bestRoad = population.PopulationList[0];
                    licznikPoprawy = 0;
                }
                else
                {
                    licznikPoprawy++;
                }
                Iterations++;
                Console.WriteLine($"Iteracja {Iterations}, current best: {bestRoad.TotalDistance}");
                z++;

            } while (licznikPoprawy <= 20);
        }

        public void MutateCCO(Population population)
        {
            _calc.OrderPopulation(population);
            int sizeOfPopulation = population.PopulationList.Count;
            Road bestRoad = population.PopulationList[0];

            for (int i = 0; i < sizeOfPopulation; i++)
            {
                string[] test = new string[population.PopulationList[0].CitiesList.Count];
                for (int j = 0; j < population.PopulationList[0].CitiesList.Count; j++)
                {
                    bool a = false;
                    for (int k = 0; k < population.PopulationList[0].CitiesList.Count; k++)
                    {
                        if (population.PopulationList[i].CitiesList[j].CityName == test[k])
                        {
                            Console.WriteLine("Population error");
                            return;
                        }
                    }
                    test[j] = population.PopulationList[i].CitiesList[j].CityName;
                }
            }

            int licznikPoprawy = 0;
            do
            {
                decimal toKill1 = Convert.ToDecimal(sizeOfPopulation) * 0.5m;
                decimal toKill2 = Convert.ToDecimal(sizeOfPopulation) - toKill1;
                population.PopulationList.RemoveRange(Convert.ToInt32(toKill1), Convert.ToInt32(toKill2));
                int z = 0;
                for (int i = population.PopulationList.Count; i < sizeOfPopulation; i++)
                {
                    int newSizeOfPopulation = population.PopulationList.Count;
                    int parent1Index = _rand.Next(0, newSizeOfPopulation);
                    int parent2Index = _rand.Next(0, newSizeOfPopulation);
                    Road parent1;
                    Road parent2;
                    parent1 = population.PopulationList[parent1Index];
                    parent2 = population.PopulationList[parent2Index];
                    int sizeOfRoad = population.PopulationList[0].CitiesList.Count;
                    Road child1 = new Road();
                    Road child2 = new Road();

                    City nullCity = new City(0, 0, "N");
                    List<City> nullCityList = new List<City>();
                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        nullCityList.Add(nullCity);
                    }
                    child1.CitiesList = nullCityList;
                    child2.CitiesList = nullCityList;

                    int indexStart = _rand.Next(0,sizeOfRoad);


                    bool backAtStart = false;
                    List<int> CitiesL1 = new List<int>();
                    List<int> CitiesL2 = new List<int>();
                    string startCity = parent1.CitiesList[indexStart].CityName;
                    string[] toDebugL1 = new string[parent1.CitiesList.Count];
                    string[] toDebugL2 = new string[parent1.CitiesList.Count];
                    for (int k = 0; k < parent1.CitiesList.Count; k++)
                    {
                        toDebugL1[k] = parent1.CitiesList[k].CityName;
                        toDebugL2[k] = parent2.CitiesList[k].CityName;
                    }
                    bool test = toDebugL1.Distinct().Count() == toDebugL1.Count();
                    if (test == false)
                    {
                        Console.WriteLine("Population generator error");
                        Console.Read();
                    }
                    bool test2 = toDebugL1.Distinct().Count() == toDebugL1.Count();
                    if (test2 == false)
                    {
                        Console.WriteLine("Population generator error");
                        Console.Read();
                    }
                    int ip1 = indexStart;
                    string sp1 = startCity;
                    CitiesL1.Add(indexStart);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    while (backAtStart == false)
                    {
                        string sp2 = parent2.CitiesList[ip1].CityName;
                        int res = 0;
                        res = parent1.CitiesList.Select((city, index) => new { city, index }).First(x => x.city.CityName == sp2).index;
                        CitiesL1.Add(res);
                        ip1 = res;
                        if (ip1 == indexStart)
                        {
                            backAtStart = true;
                            continue;
                        }
                    }

                    string[] toDebugL1B = new string[parent1.CitiesList.Count];
                    string[] toDebugL2B = new string[parent1.CitiesList.Count];

                    for (int j = 0; j < CitiesL1.Count; j++)
                    {
                        child1.CitiesList[CitiesL1[j]] = parent1.CitiesList[CitiesL1[j]];
                        toDebugL1B[CitiesL1[j]] = parent1.CitiesList[CitiesL1[j]].CityName;
                    }
                    for (int j = 0; j < CitiesL1.Count; j++)
                    {
                        child2.CitiesList[CitiesL1[j]] = parent1.CitiesList[CitiesL1[j]];
                        toDebugL2B[CitiesL1[j]] = parent1.CitiesList[CitiesL1[j]].CityName;
                    }
                    for (int j = 0; j < sizeOfRoad; j++)
                    {
                        if (child1.CitiesList[j].CityName == "N")
                        {
                            child1.CitiesList[j] = parent2.CitiesList[j];
                            toDebugL1B[j] = parent2.CitiesList[j].CityName;
                        }
                        if (child2.CitiesList[j].CityName == "N")
                        {
                            child2.CitiesList[j] = parent1.CitiesList[j];
                            toDebugL2B[j] = parent1.CitiesList[j].CityName;
                        }
                    }
                    string[] toDebugL1C = new string[parent1.CitiesList.Count];
                    string[] toDebugL2C = new string[parent1.CitiesList.Count];
                    for (int k = 0; k < parent1.CitiesList.Count; k++)
                    {
                        toDebugL1C[k] = child1.CitiesList[k].CityName;
                        toDebugL2C[k] = child2.CitiesList[k].CityName;
                    }
                    bool testC = toDebugL1C.Distinct().Count() == toDebugL1C.Count();
                    if (testC == false)
                    {
                        Console.WriteLine("Population generator error");
                        Console.Read();
                    }
                    bool test2C = toDebugL1C.Distinct().Count() == toDebugL1C.Count();
                    if (testC == false)
                    {
                        Console.WriteLine("Population generator error");
                        Console.Read();
                    }
                    _calc.CalculateTotalDistance(child1, cykl);
                    _calc.CalculateTotalDistance(child2, cykl);

                    population.PopulationList.Add(child1);
                    population.PopulationList.Add(child2);

                }
                _calc.OrderPopulation(population);
                decimal oldBest = bestRoad.TotalDistance;

                if (population.PopulationList[0].TotalDistance < bestRoad.TotalDistance)
                {
                    bestRoad = population.PopulationList[0];
                    licznikPoprawy = 0;
                }
                else
                {
                    licznikPoprawy++;
                }
                Iterations++;
                Console.WriteLine($"Iteration no. {Iterations}, current best: {bestRoad.TotalDistance}");
                z++;
            } while (licznikPoprawy <= 31);
            Console.WriteLine(RandomMutations);
        }

    }
}

