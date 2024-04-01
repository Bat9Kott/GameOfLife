using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Drawing;

namespace GameOfLife
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graphics graph;

        private PictureBox PictureSpace = new PictureBox();

        private int resolution;

        DispatcherTimer timer = new DispatcherTimer();

        private bool[,] fieldB;
        private bool[,] fieldR;
        private int rows;
        private int cols;

        public MainWindow()
        {
            InitializeComponent();
            StartForm();
        }

        private void StartForm()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            timer.Tick += new EventHandler(timer_Tick);
            
            PictureSpace.Size = new System.Drawing.Size((int)Window.Width, (int)Window.Height);
        }

        private void StartGame()
        {
            if (timer.IsEnabled)
                return;
            PlusR.IsEnabled = false;
            PlusD.IsEnabled = false;
            MinusR.IsEnabled = false;
            MinusD.IsEnabled = false;

            resolution = Convert.ToInt32(nudResol.Content);
            
            rows = PictureSpace.Height / resolution;
            cols = PictureSpace.Width / resolution;
            fieldR = new bool[cols, rows];
            fieldB = new bool[cols, rows];

            Random rand = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    fieldR[x,y] = rand.Next(Convert.ToInt32(nudDens.Content)) == 0;
                    fieldB[x, y] = rand.Next(Convert.ToInt32(nudDens.Content)) == 0;
                }
            }

            PictureSpace.Image = new Bitmap(PictureSpace.Width, PictureSpace.Height);
            graph = Graphics.FromImage(PictureSpace.Image);
            graph.Clear(Color.Black);
            WFHost.Child = PictureSpace;
            timer.Start();
        }

        private bool ChechRules(bool[,] a, int x, int y, bool isLive)
        {
            int neigh = CountNeigh(x, y);
            if (!isLive && neigh == 3)
                return true;
            else if (isLive && (neigh < 2 || neigh > 3))               //ПРОВЕРИТЬ-НОРМАЛЬНО ЛИ РАБОТАЕТ!!!
                return false;
            else
                return a[x, y];
        }

        private void NextGen()
        {
            graph.Clear(Color.Black);

            bool[,] newGenR = new bool[cols, rows];
            bool[,] newGenB = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    newGenR[x,y]=ChechRules(fieldR, x, y, fieldR[x, y]);

                    newGenB[x, y] = ChechRules(fieldB, x, y, fieldB[x, y]);

                    if (fieldR[x, y])
                        graph.FillRectangle(Brushes.Red, x*resolution, y*resolution, resolution-1, resolution-1);
                    if (fieldB[x, y])
                        graph.FillRectangle(Brushes.Blue, x * resolution, y * resolution, resolution - 1, resolution - 1);
                }
            }
            fieldR = newGenR;
            fieldB = newGenB;
            PictureSpace.Refresh();
        }

        //private int CountNeigh(int x, int y)
        //{
        //    int count = 0;
        //    for (int i = -1; i < 2; i++)
        //    {
        //        for (int j = -1; j < 2; j++)
        //        {
        //            int col = x + j;
        //            int row = y + i;
        //            bool SelfCheck = row == y && col == x;
        //        }
        //    }

        //    return 0;
        //}

        private void StopGame()
        {
            if (!timer.IsEnabled)
                return;
            timer.Stop();
            PlusR.IsEnabled = true;
            PlusD.IsEnabled = true;
            MinusR.IsEnabled = true;
            MinusD.IsEnabled = true;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            NextGen();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
        }

        private void PlusNumUD(System.Windows.Controls.Button a)
        {
            if (Convert.ToInt32(a.Content) >= 1 && Convert.ToInt32(a.Content) <= 24)
                a.Content = (Convert.ToInt32(a.Content) + 1).ToString();
        }

        private void MinusNumUD(System.Windows.Controls.Button a)
        {
            if (Convert.ToInt32(a.Content) > 2 && Convert.ToInt32(a.Content) <= 25)
                a.Content = (Convert.ToInt32(a.Content) - 1).ToString();
        }

        private void PlusR_Click(object sender, RoutedEventArgs e)
        {
            PlusNumUD(nudResol);
        }

        private void MinusR_Click(object sender, RoutedEventArgs e)
        {
            MinusNumUD(nudResol);
        }

        private void PlusD_Click(object sender, RoutedEventArgs e)
        {
            PlusNumUD(nudDens);
        }

        private void MinusD_Click(object sender, RoutedEventArgs e)
        {
            MinusNumUD(nudDens);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
