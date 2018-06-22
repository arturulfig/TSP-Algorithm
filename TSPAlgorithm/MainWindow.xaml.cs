using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TSPAlgorithm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int populationSize = 5000;
        List<City> CitiesList = new List<City>();
        bool CityListIsLoaded = false;
        string Met;
        public MainWindow()
        {
            InitializeComponent();
            PopulationSizeBox.MaxLength = 10;
            Met = MethodBox.Text;
        }
        private void CalculateBtn(object sender, RoutedEventArgs e)
        {
            ScoreBoard.Text = "Calculating...";
            if (CityListIsLoaded == false)
            {
                ScoreBoard.Text = $"{ScoreBoard.Text}\r\nCities loaded successfully";
            }
            var rand = new Random();
            Met = MethodBox.Text;
            Console.WriteLine(Met);

            if (CityListIsLoaded == false)
            {
                CitiesList.Add(new City(37, 79, "0"));
                CitiesList.Add(new City(-90, -44, "1"));
                CitiesList.Add(new City(-89, 13, "2"));
                CitiesList.Add(new City(38, -66, "3"));
                CitiesList.Add(new City(36, -100, "4"));
                CitiesList.Add(new City(38, 41, "5"));
                CitiesList.Add(new City(-31, 74, "6"));
                CitiesList.Add(new City(61, 20, "7"));
                CitiesList.Add(new City(-29, 39, "8"));
                CitiesList.Add(new City(71, -85, "9"));
                CitiesList.Add(new City(12, -2, "10"));
                CitiesList.Add(new City(-41, 17, "11"));
                CitiesList.Add(new City(17, -36, "12"));
                CitiesList.Add(new City(91, -66, "13"));
                CitiesList.Add(new City(71, -70, "14"));
                CitiesList.Add(new City(74, -21, "15"));
                CitiesList.Add(new City(16, -74, "16"));
                CitiesList.Add(new City(-56, -67, "17"));
                CitiesList.Add(new City(63, 15, "18"));
                CitiesList.Add(new City(-55, 26, "19"));
                CitiesList.Add(new City(-38, 26, "20"));
                CitiesList.Add(new City(8, 59, "21"));
                CitiesList.Add(new City(91, -86, "22"));
                CitiesList.Add(new City(-99, -12, "23"));
                CitiesList.Add(new City(-96, -58, "24"));
            }

            foreach (var item in CitiesList)
            {
                Console.WriteLine($"{item.X} \t{item.Y} \t{item.CityName}");
            }

            CalculateFunction calc = new CalculateFunction();


            var pop = new Population();

            bool cycle = false;

            if (ClCyclCheck.IsChecked == true)
            {
                cycle = true;
            }
            var mut = new Mutation(cycle);

            int finalPopulationSize = populationSize;

            mut.GenerateRandomPopulation(pop, CitiesList, populationSize);
            calc.CalculatePopulationDistances(pop, cycle);
            calc.OrderPopulation(pop);
            Stopwatch sw = new Stopwatch();

            if (Met == "OX1")
            {
                sw.Start();
                mut.MutateOX1(pop, CitiesList);
                sw.Stop();
            }
            else if (Met == "ERO")
            {
                sw.Start();
                mut.MutateERO(pop, CitiesList);
                sw.Stop();
            }
            else if (Met == "CCO")
            {
                sw.Start();
                mut.MutateCCO(pop);
                sw.Stop();
            }

            string Score = $"Iterations: {mut.Iterations} Elapsed Time {sw.Elapsed}\r\n" +
                $"Optimal score: {pop.PopulationList[0].TotalDistance}\r\n";
            var lista = pop.PopulationList[0].CitiesList;
           
            ScoreBoard.Text = Score;

            Console.WriteLine(ScoreBoard);

            

            Bitmap bmp = new Bitmap(10000, 10000);
            System.Drawing.Pen blackPen = new System.Drawing.Pen(System.Drawing.Color.Red, 10);

            for (int i = 0; i < pop.PopulationList[0].CitiesList.Count; i++)
            {
                if (cycle == false && i == pop.PopulationList[0].CitiesList.Count - 1)
                {
                    continue;
                }
                else if (cycle == true && i == pop.PopulationList[0].CitiesList.Count - 1)
                {
                    int x1b = (int)pop.PopulationList[0].CitiesList[i].X + 100;
                    int x2b = (int)pop.PopulationList[0].CitiesList[0].X + 100;
                    int y1b = (int)pop.PopulationList[0].CitiesList[i].Y + 100;
                    int y2b = (int)pop.PopulationList[0].CitiesList[0].Y + 100;
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        graphics.DrawLine(blackPen, x1b, y1b, x2b, y2b);
                    }
                    continue;
                }
                int x1 = (int)pop.PopulationList[0].CitiesList[i].X + 100;
                int x2 = (int)pop.PopulationList[0].CitiesList[i + 1].X + 100;
                int y1 = (int)pop.PopulationList[0].CitiesList[i].Y + 100;
                int y2 = (int)pop.PopulationList[0].CitiesList[i + 1].Y + 100;

                using (var graphics = Graphics.FromImage(bmp))
                {
                    graphics.DrawLine(blackPen, x1, y1, x2, y2);
                }
            }

            bmp.Save("map.bmp");
        }
        private void ImageLoad( object sender, RoutedEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri("C:\map.bmp");
            b.EndInit();
        }
        private void PopulationSizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PopulationSizeBox.Text.Length == 0)
            {
                populationSize = 5000;
                return;
            }
            string popSizeString = PopulationSizeBox.Text;
            populationSize = Convert.ToInt32(popSizeString);
            Console.WriteLine(populationSize);

        }

        private void PopulationSiz_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[0-9]");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void LoadCoordsBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadCities load = new LoadCities();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files (*.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                CitiesList = load.ReadCitiesLocationFromFile(filename);
                CityListIsLoaded = true;
            }

        }

    }
}
