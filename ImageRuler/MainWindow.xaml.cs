using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageRuler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum RulerMode
    {
        None,
        SetXstart,
        SetXend,
        SetYstart,
        SetYend,
        Mark
    }

    public partial class MainWindow : Window, DataObserver
    {
        DataWindow dataWindow = new DataWindow();
        InputWindow inputWindow = new InputWindow();
        RulerMode mode;
        Axis xAxis = new Axis();
        Axis yAxis = new Axis();
        int width = 0;
        int height = 0;

        double zoom = 1;
        double zoomAmount = Math.Sqrt(2);
        Point lastPoint = new Point();
        bool lastPointSet = false;


        public MainWindow()
        {
            InitializeComponent();

            dataWindow.registerObserver(this);
        }

        public void resetData()
        {
            resetVisual();
        }

        void reset()
        {
            dataWindow.resetData();
            resetVisual();
            xAxis.reset();
            yAxis.reset();
            zoom = 1;
            lastPoint = new Point();
            lastPointSet = false;
        }

        void resetVisual()
        {
            imageGrid.Children.Clear();
        }

        public static BitmapSource loadBitmapImage(string fileName)
        {
            if (fileName != "")
            {
                try
                {
                    BitmapImage source = new BitmapImage();
                    source.BeginInit();
                    source.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.UriSource = new Uri(fileName, UriKind.Absolute);
                    source.EndInit();
                    return source;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        private void clipboardImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            setNewImage(Clipboard.GetImage());
        }

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open image";

            if (openFileDialog.ShowDialog() == true)
            {
                setNewImage(loadBitmapImage(openFileDialog.FileName));
            }
        }

        void setNewImage(BitmapSource bitmap)
        {
            if (bitmap != null)
            {
                width = bitmap.PixelWidth;
                height = bitmap.PixelHeight;
                mainImage.Source = bitmap;
                mainImage.Width = width;
                mainImage.Height = height;

                reset();
                mode = RulerMode.SetXstart;
                updateMode();
            }
        }

        private void exportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export data";
            saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                dataWindow.exportData(saveFileDialog.FileName);
            }
        }

        private void xRulerStartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mode = RulerMode.SetXstart;
            updateMode();
        }

        private void xRulerEndMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mode = RulerMode.SetXend;
            updateMode();
        }

        private void yRulerStartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mode = RulerMode.SetYstart;
            updateMode();
        }

        private void yRulerEndMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mode = RulerMode.SetYend;
            updateMode();
        }

        private void markMenuItem_Click(object sender, RoutedEventArgs e)
        {
            mode = RulerMode.Mark;
            updateMode();
        }

        void updateMode()
        {
            xRulerStartMenuItem.IsChecked = (mode == RulerMode.SetXstart);
            xRulerEndMenuItem.IsChecked = (mode == RulerMode.SetXend);
            yRulerStartMenuItem.IsChecked = (mode == RulerMode.SetYstart);
            yRulerEndMenuItem.IsChecked = (mode == RulerMode.SetYend);
            markMenuItem.IsChecked = (mode == RulerMode.Mark);

            switch (mode)
            {
                case RulerMode.SetXstart: statusText.Content = "Setting X ruler start"; break;
                case RulerMode.SetXend: statusText.Content = "Setting X ruler end"; break;
                case RulerMode.SetYstart: statusText.Content = "Setting Y ruler start"; break;
                case RulerMode.SetYend: statusText.Content = "Setting Y ruler end"; break;
            }
        }

        private void mainImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(mainImage);
            double? value;

            switch (mode)
            {
                case RulerMode.SetXstart:
                    value = inputWindow.getInput("X start", xAxis.value0);
                    if (value != null)
                    {
                        xAxis.setStart(pos.X, (double)value);
                        mode = RulerMode.SetXend;
                        updateMode();                        
                    }
                    break;

                case RulerMode.SetXend:
                    value = inputWindow.getInput("X end", xAxis.value1);
                    if (value != null)
                    {
                        xAxis.setEnd(pos.X, (double)value);
                        mode = RulerMode.SetYstart;
                        updateMode();
                    }
                    break;

                case RulerMode.SetYstart:
                    value = inputWindow.getInput("Y start", yAxis.value0);
                    if (value != null)
                    {
                        yAxis.setStart(pos.Y, (double)value);
                        mode = RulerMode.SetYend;
                        updateMode();
                    }
                    break;

                case RulerMode.SetYend:
                    value = inputWindow.getInput("Y end", yAxis.value1);
                    if (value != null)
                    {
                        yAxis.setEnd(pos.Y, (double)value);
                        mode = RulerMode.Mark;
                        updateMode();
                    }
                    break;

                case RulerMode.Mark:
                    mark(pos);
                    updatePosition(pos);
                    break;
            }
        }

        void mark(Point pos)
        {
            double x, y;

            if (xAxis.isSet() || yAxis.isSet())
            {
                x = xAxis.getValue(pos.X);
                y = yAxis.getValue(pos.Y);

                dataWindow.addData(x, y);
                drawPoint(pos);

                lastPoint = new Point(x, y);
                lastPointSet = true;
            }
        }

        void drawPoint(Point pos)
        {
            Line line;

            line = new Line();
            line.Stroke = Brushes.Gray;
            line.StrokeThickness = 1;
            line.X1 = pos.X - 1;
            line.X2 = pos.X + 1;
            line.Y1 = pos.Y;
            line.Y2 = pos.Y;
            imageGrid.Children.Add(line);

            line = new Line();
            line.Stroke = Brushes.Gray;
            line.StrokeThickness = 1;
            line.X1 = pos.X;
            line.X2 = pos.X;
            line.Y1 = pos.Y - 1;
            line.Y2 = pos.Y + 1;
            imageGrid.Children.Add(line);
        }

        private void mainImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point pos = e.GetPosition(mainImage);
            updatePosition(pos);
        }

        void updatePosition(Point pos)
        {
            string s = "";
            double x, y;
            double dx = 0;
            double dy = 0;

            if (mode == RulerMode.Mark)
            {
                if (xAxis.isSet())
                {
                    x = xAxis.getValue(pos.X);
                    dx = x - lastPoint.X;
                    s += string.Format("X: {0}", x);
                }
                if (yAxis.isSet())
                {
                    if (s != "")
                    {
                        s += " ";
                    }
                    y = yAxis.getValue(pos.Y);
                    dy = y - lastPoint.Y;
                    s += string.Format("Y: {0}", y);
                }
                if (lastPointSet)
                {
                    if (dx != 0 || dy != 0)
                    {
                        s += " (";
                    }
                    if (dx != 0)
                    {
                        s += string.Format(" ΔX: {0}", dx);
                    }
                    if (dy != 0)
                    {
                        s += string.Format(" ΔY: {0}", dy);
                    }
                    if (dx != 0 || dy != 0)
                    {
                        s += ")";
                    }
                }
                statusText.Content = s;
            }
        }

        private void showDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            dataWindow.Hide();
            dataWindow.Show();
        }

        private void clearDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            dataWindow.resetData();
            lastPoint = new Point();
            lastPointSet = false;
        }

        private void zoomResetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            zoom = 1;
            updateZoom();
        }

        private void zoomInMenuItem_Click(object sender, RoutedEventArgs e)
        {
            zoom *= zoomAmount;
            updateZoom();
        }

        private void zoomOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            zoom /= zoomAmount;
            updateZoom();
        }

        private void mainImage_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            double zoomf = Math.Pow(zoomAmount, e.Delta / 120);

            zoom *= zoomf;
            updateZoom();
        }

        void updateZoom()
        {
            gridScale.ScaleX = zoom;
            gridScale.ScaleY = zoom;
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }

    }
}
