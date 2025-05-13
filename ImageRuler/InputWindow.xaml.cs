using System.Windows;

namespace ImageRuler
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        bool ok = false;

        public InputWindow()
        {
            InitializeComponent();
        }

        public double? getInput(string caption, double defValue)
        {
            Title = caption;
            inputText.Text = defValue.ToString();
            ok = false;
            ShowDialog();
            if (ok)
            {
                return int.Parse(inputText.Text);
            }
            return null;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            ok = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            inputText.SelectAll();
            inputText.Focus();
        }

    }
}
