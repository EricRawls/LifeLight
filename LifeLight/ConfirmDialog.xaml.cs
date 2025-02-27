using System.Windows;

namespace LifeLight
{
    public partial class ConfirmDialog : Window
    {
        public string Message { get; set; }
        public string TitleText { get; set; } // New property for the title
        public bool Result { get; private set; }

        public ConfirmDialog(Window owner, string message, string title)
        {
            InitializeComponent();
            Owner = owner;
            Message = message;
            TitleText = title; // Set the title
            DataContext = this; // Bind both Message and TitleText to the UI
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = false;
            Close();
        }
    }
}