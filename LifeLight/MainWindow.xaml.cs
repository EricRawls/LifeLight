using LifeLight;
using System.Collections.ObjectModel;
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
    public partial class MainWindow : Window
    {
        public ObservableCollection<DailyTodoItem> ocDailyNeeds { get; set; }
        public ObservableCollection<VariableTodoItem> ocVariableNeeds { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo));

            //DateTime today = DateTime.Today;
            //calDate.BlackoutDates.Add(new CalendarDateRange(today.AddDays(1), today.AddYears(1)));
            ocVariableNeeds = new ObservableCollection<VariableTodoItem>
            {
                new VariableTodoItem() { Title = "Poot" },
                new VariableTodoItem() { Title = "Fart" },
                new VariableTodoItem() { Title = "Shart" }
            };

            ocDailyNeeds = new ObservableCollection<DailyTodoItem>
                {
                new DailyTodoItem() { Title = "Morning Benzodiazepine", TimeVisibility = Visibility.Visible },
                new DailyTodoItem() { Title = "AM Vitamins" },
                new DailyTodoItem() { Title = "Breakfast", TimeVisibility = Visibility.Visible },
                new DailyTodoItem() { Title = "AM Brush Teeth / Mouthwash" },
                new DailyTodoItem() { Title = "Midday Benzodiazepine", TimeVisibility = Visibility.Visible },
                new DailyTodoItem() { Title = "Lunch", TimeVisibility = Visibility.Visible },
                new DailyTodoItem() { Title = "Brush Hair" },
                new DailyTodoItem() { Title = "Exercise" },
                new DailyTodoItem() { Title = "Supper", TimeVisibility = Visibility.Visible },
                new DailyTodoItem() { Title = "PM Vitamins" },
                new DailyTodoItem() { Title = "Night Benzodiazepine", TimeVisibility = Visibility.Visible },
                new DailyTodoItem() { Title = "PM Brush Teeth / Mouthwash" }
            };

            lvDailyNeeds.ItemsSource = ocDailyNeeds;
            lvVariableNeeds.ItemsSource = ocVariableNeeds;
        }

        private void btnAppTODO_Click(object sender, RoutedEventArgs e)
        {
            var atdWindow = new AppTODOWindow { Owner = this };
            atdWindow.Show();
        }

        private void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.CanRedo)
            {
                textBox.Redo();
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void tbTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox tb && tb.DataContext is DailyTodoItem item)
                {
                    tbTime_Validate(tb, item);
                    e.Handled = true; // Prevent further processing (e.g., newline)
                }
            }
        }

        private void tbTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is DailyTodoItem item)
            {
                tbTime_Validate(tb, item);
            }
        }

        private void tbTime_Validate(TextBox tb, DailyTodoItem item)
        {
            if (DateTime.TryParse(tb.Text, out DateTime time))
            {
                item.Time = time;
                // Optionally format for display (but keep raw for editing)
                tb.Text = time.ToString("hh:mm tt");
            }
            else
            {
                MessageBox.Show("Please enter a valid time (e.g., 01:30 PM).", "Invalid Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb.Text = item.Time.ToString("hh:mm tt"); // Revert to last valid value
            }
        }

        private void cbComplete_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("CheckBox clicked!");
            if (sender is CheckBox cb && cb.DataContext is DailyTodoItem item)
            {
                item.Complete = cb.IsChecked ?? false; // Update Complete based on IsChecked
            }
        }

        private void AddAboveDaily_Click(object sender, RoutedEventArgs e)
        {
            int index = lvDailyNeeds.SelectedIndex;
            if (index >= 0)
            {
                ShowAddDailyItemWindow(index);
            }
            else
            {
                ShowAddDailyItemWindow(0); // Default to top if nothing selected
            }
        }

        private void AddBelowDaily_Click(object sender, RoutedEventArgs e)
        {
            int index = lvDailyNeeds.SelectedIndex;
            if (index >= 0)
            {
                ShowAddDailyItemWindow(index + 1);
            }
            else
            {
                ShowAddDailyItemWindow(ocDailyNeeds.Count); // Default to end if nothing selected
            }
        }

        private void EditDailyItem_Click(object sender, RoutedEventArgs e)
        {
            int index = lvDailyNeeds.SelectedIndex;
            if (index >= 0)
            {
                var editWindow = new EditDailyItemWindow(ocDailyNeeds[index]) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    // Update the existing item with new values
                    ocDailyNeeds[index].Title = editWindow.NewTitle!;
                    ocDailyNeeds[index].TimeVisibility = editWindow.NewTimeVisibility;
                }
            }
        }
        private void DeleteDailyItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvDailyNeeds.SelectedItem is DailyTodoItem selectedItem)
            {
                var dialog = new ConfirmDialog(this, "Are you sure you want to delete this item?", "Confirm Deletion");
                if (dialog.ShowDialog() == true)
                {
                    ocDailyNeeds.Remove(selectedItem);
                }
            }
        }

        private void SortDailyItems_Click(object sender, RoutedEventArgs e)
        {
            var sortWindow = new SortTodoItemsWindow(new ObservableCollection<DailyTodoItem>(ocDailyNeeds)) { Owner = this };
            if (sortWindow.ShowDialog() == true)
            {
                ocDailyNeeds.Clear();
                foreach (var item in sortWindow.SortedItems)
                {
                    ocDailyNeeds.Add(item);
                }
            }
        }

        private void ShowAddDailyItemWindow(int insertIndex)
        {
            var addWindow = new AddDailyItemWindow { Owner = this };
            if (addWindow.ShowDialog() == true)
            {
                ocDailyNeeds.Insert(insertIndex >= 0 ? insertIndex : ocDailyNeeds.Count,
                    new DailyTodoItem
                    {
                        Title = addWindow.NewTitle!,
                        TimeVisibility = addWindow.NewTimeVisibility
                    });
            }
        }

        private void btnSetNow_Click(object sender, RoutedEventArgs e)
        {
            if (lvDailyNeeds.SelectedIndex >= 0)
            {
                var selectedItem = ocDailyNeeds[lvDailyNeeds.SelectedIndex];
                selectedItem.Time = DateTime.Now;
                //MessageBox.Show($"Time set to: {selectedItem.Time.ToString("hh:mm tt")}");
            }
        }

        private void calDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calDate.SelectedDate.HasValue)
            {
                //MessageBox.Show($"Selected Date: {calDate.SelectedDate.Value.ToString("d")}");
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export clicked!");
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolean)
                return !boolean;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolean)
                return !boolean;
            return value;
        }
    }

}

public class DailyTodoItem : INotifyPropertyChanged
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

public class VariableTodoItem : INotifyPropertyChanged
{
    private string _title = "";
    private bool _complete;
    private DateTime _date;
    private int _daysFrequency = 1;
    private string _comment = "";
    private Visibility _dueDateVisibility = Visibility.Hidden;

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
        get => _date;
        set { _date = value; OnPropertyChanged(); }
    }
    public string Comment
    {
        get => _comment;
        set { _comment = value; OnPropertyChanged(); }
    }
    public int DaysFrequency
    {
        get => _daysFrequency;
        set { _daysFrequency = value; OnPropertyChanged(); }
    }
    public Visibility DueDateVisibility
    {
        get => _dueDateVisibility;
        set { _dueDateVisibility = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
