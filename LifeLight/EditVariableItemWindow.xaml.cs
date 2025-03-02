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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LifeLight
{
    public partial class EditVariableItemWindow : Window
    {
        public string? NewTitle { get; private set; }
        public Visibility NewDueDateVisibility { get; private set; }
        public int NewDaysFrequency { get; private set; }

        public EditVariableItemWindow(VariableTodoItem item)
        {
            InitializeComponent();

            // Populate controls with existing data
            txtTitle.Text = item.Title;
            txtFrequency.Text = item.DaysFrequency.ToString();

            // Set initial return values
            NewTitle = item.Title;
            NewDueDateVisibility = item.DueDateVisibility;
            NewDaysFrequency = item.DaysFrequency;

        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                NewTitle = txtTitle.Text;
                if (int.TryParse(txtFrequency.Text, out int frequency) && frequency > 0)
                {
                    NewDaysFrequency = frequency;
                    NewDueDateVisibility = Visibility.Visible;
                }
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

        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
            if (!IsValidInteger(newText))
            {
                e.Handled = true;
            }
        }

        private void IntegerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Tag = IsValidInteger(textBox.Text) ? null : "Invalid";
        }

        private bool IsValidInteger(string text)
        {
            if (string.IsNullOrEmpty(text) || text == "-") return true;
            return int.TryParse(text, out _);
        }
    }
}
