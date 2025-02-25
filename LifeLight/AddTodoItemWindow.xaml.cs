using System.Windows;
using System.Windows.Input;

namespace LifeLight // Match your project’s namespace
{
    public partial class AddTodoItemWindow : Window
    {
        public string? NewTitle { get; private set; }

        public AddTodoItemWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                NewTitle = txtTitle.Text;
                DialogResult = true; // Signals OK was clicked
                Close();
            }
            else
            {
                MessageBox.Show("Title is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Signals cancellation
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