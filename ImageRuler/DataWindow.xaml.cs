using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ImageRuler
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>

    public interface DataObserver
    {
        void resetData();
    }

    public partial class DataWindow : Window
    {
        List<DataObserver> observers = new List<DataObserver>();

        List<Point> points = new List<Point>();


        public DataWindow()
        {
            InitializeComponent();
        }

        public void registerObserver(DataObserver observer)
        {
            observers.Add(observer);
        }

        public void unregisterObserver(DataObserver observer)
        {
            observers.Remove(observer);
        }

        public void resetData()
        {
            points.Clear();
            redraw();
        }

        public void addData(double x, double y)
        {
            points.Add(new Point(x, y));
            redraw();
        }

        void redraw()
        {
            string s = "";
            foreach (Point point in points)
            {
                s += string.Format("{0},{1}\n", point.X, point.Y);
            }
            dataText.Text = s;
        }

        public void exportData(string filename)
        {
            StreamWriter writer = new StreamWriter(filename);
            foreach (Point point in points)
            {
                writer.WriteLine(string.Format("{0},{1}", point.X, point.Y));
            }
            writer.Close();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            resetData();
            foreach (DataObserver observer in observers)
            {
                observer.resetData();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

    }
}
