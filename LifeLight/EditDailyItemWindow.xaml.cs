using System.Windows;
using System.Windows.Input;

namespace LifeLight
{
    /// <summary>
    /// Interaction logic for EditDailyItemWindow.xaml
    /// </summary>
    public partial class EditDailyItemWindow : Window
    {
        public string? NewTitle { get; private set; }
        public Visibility NewTimeVisibility { get; private set; }
        private DailyTodoItem _originalItem;

        // Constructor that accepts the item to edit
        public EditDailyItemWindow(DailyTodoItem item)
        {
            InitializeComponent();
            _originalItem = item;

            // Populate controls with existing data
            txtTitle.Text = item.Title;
            cbShowTime.IsChecked = item.TimeVisibility == Visibility.Visible;

            // Set initial return values
            NewTitle = item.Title;
            NewTimeVisibility = item.TimeVisibility;
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
