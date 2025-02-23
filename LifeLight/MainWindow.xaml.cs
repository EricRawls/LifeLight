using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            Items.Add(new TodoItem() { Title = "Morning Benzodiazepine", TimeVisibility = Visibility.Visible });
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
            
            lbTodoList.ItemsSource = Items;
        }

        private void btnSetNow_Click(object sender, RoutedEventArgs e)
        {
            Items[lbTodoList.SelectedIndex].Time = DateTime.Now;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //Do stuff
        }

        private void calDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(calDate.SelectedDate.ToString());
        }

    }

}

public class TodoItem : INotifyPropertyChanged
{
    private string _title = "";
    private bool _complete;
    private DateTime _time;
    private string _comment = "";
    private Visibility _timeVisibility = Visibility.Hidden;

    public required string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }
    public bool Complete
    {
        get => _complete;
        set { _complete = value; OnPropertyChanged(); }
    }
    public DateTime Time
    {
        get => _time;
        set { _time = value; OnPropertyChanged(); }
    }
    public string Comment
    {
        get => _comment;
        set { _comment = value; OnPropertyChanged(); }
    }
    public Visibility TimeVisibility
    {
        get => _timeVisibility;
        set { _timeVisibility = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


