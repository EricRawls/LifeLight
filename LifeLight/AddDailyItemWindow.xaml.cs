using System.Windows;
using System.Windows.Input;

namespace LifeLight
{
    public partial class AddDailyItemWindow : Window
    {
        public string? NewTitle { get; private set; }
        public Visibility NewTimeVisibility { get; private set; }

        public AddDailyItemWindow()
        {
            InitializeComponent();
            NewTimeVisibility = Visibility.Hidden; // Default value
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                NewTitle = txtTitle.Text;
                NewTimeVisibility = cbShowTime.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Title is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TxtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OK_Click(sender, e);
            }
        }
    }
}