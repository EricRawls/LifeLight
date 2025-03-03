using System.Windows;
using System.Windows.Input;

namespace LifeLight
{
    public partial class EditDailyItemWindow : Window
    {
        public string? NewTitle { get; private set; }
        public Visibility NewTimeVisibility { get; private set; }

        public EditDailyItemWindow(DailyTodoItem item)
        {
            InitializeComponent();

            txtTitle.Text = item.Title;
            cbShowTime.IsChecked = item.TimeVisibility == Visibility.Visible;

            NewTitle = item.Title;
            NewTimeVisibility = item.TimeVisibility;

            txtTitle.SelectionStart = txtTitle.Text.Length;
            txtTitle.Focus();
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

    }
}
