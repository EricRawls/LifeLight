using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TodoItem> Items;
        public MainWindow()
        {
            InitializeComponent();

            Items = new List<TodoItem>();
            Items.Add(new TodoItem() { Title = "Morning Benzodiazepine", TimeVisibility = Visibility.Visible, Comment = "POOT" });
            Items.Add(new TodoItem() { Title = "AM Vitamins" });
            Items.Add(new TodoItem() { Title = "Breakfast", TimeVisibility = Visibility.Visible });
            Items.Add(new TodoItem() { Title = "AM Brush Teeth / Mouthwash" });
            Items.Add(new TodoItem() { Title = "Midday Benzodiazepine", TimeVisibility = Visibility.Visible });
            Items.Add(new TodoItem() { Title = "Lunch", TimeVisibility = Visibility.Visible });
            Items.Add(new TodoItem() { Title = "Brush Hair" });
            Items.Add(new TodoItem() { Title = "Exercise" });
            Items.Add(new TodoItem() { Title = "Supper", TimeVisibility = Visibility.Visible });
            Items.Add(new TodoItem() { Title = "PM Vitamins" });
            Items.Add(new TodoItem() { Title = "Night Benzodiazepine", TimeVisibility = Visibility.Visible });
            Items.Add(new TodoItem() { Title = "PM Brush Teeth / Mouthwash" });
            
            TodoList.ItemsSource = Items;
        }

        private void btnSetNow_Click(object sender, RoutedEventArgs e)
        {
            Items[TodoList.SelectedIndex].Time = DateTime.Now;
            MessageBox.Show(Items[TodoList.SelectedIndex].Time.ToString()); 
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in TodoList.SelectedItems)
                MessageBox.Show((o as TodoItem).Title);
        }

        private void calDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(calDate.SelectedDate.ToString());
        }

    }

    public class TodoItem
    {
        public string Title { get; set; }
        public bool Complete { get; set; }
        public DateTime Time { get; set; }
        public string Comment { get; set; }
        public Visibility TimeVisibility { get; set; } = Visibility.Hidden;
    }
}