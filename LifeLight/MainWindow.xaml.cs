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
        public ObservableCollection<TodoItem> Items { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo));

            //DateTime today = DateTime.Today;
            //calDate.BlackoutDates.Add(new CalendarDateRange(today.AddDays(1), today.AddYears(1)));

            Items = new ObservableCollection<TodoItem>
                {
                new TodoItem() { Title = "Morning Benzodiazepine", TimeVisibility = Visibility.Visible },
                new TodoItem() { Title = "AM Vitamins" },
                new TodoItem() { Title = "Breakfast", TimeVisibility = Visibility.Visible },
                new TodoItem() { Title = "AM Brush Teeth / Mouthwash" },
                new TodoItem() { Title = "Midday Benzodiazepine", TimeVisibility = Visibility.Visible },
                new TodoItem() { Title = "Lunch", TimeVisibility = Visibility.Visible },
                new TodoItem() { Title = "Brush Hair" },
                new TodoItem() { Title = "Exercise" },
                new TodoItem() { Title = "Supper", TimeVisibility = Visibility.Visible },
                new TodoItem() { Title = "PM Vitamins" },
                new TodoItem() { Title = "Night Benzodiazepine", TimeVisibility = Visibility.Visible },
                new TodoItem() { Title = "PM Brush Teeth / Mouthwash" }
            };

            lvTodoList.ItemsSource = Items;
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
                if (sender is TextBox tb && tb.DataContext is TodoItem item)
                {
                    tbTime_Validate(tb, item);
                    e.Handled = true; // Prevent further processing (e.g., newline)
                }
            }
        }

        private void tbTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is TodoItem item)
            {
                tbTime_Validate(tb, item);
            }
        }

        private void tbTime_Validate(TextBox tb, TodoItem item)
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
            if (sender is CheckBox cb && cb.DataContext is TodoItem item)
            {
                item.Complete = cb.IsChecked ?? false; // Update Complete based on IsChecked
            }
        }

        private void AddAbove_Click(object sender, RoutedEventArgs e)
        {
            int index = lvTodoList.SelectedIndex;
            if (index >= 0)
            {
                ShowAddItemWindow(index);
            }
            else
            {
                ShowAddItemWindow(0); // Default to top if nothing selected
            }
        }

        private void AddBelow_Click(object sender, RoutedEventArgs e)
        {
            int index = lvTodoList.SelectedIndex;
            if (index >= 0)
            {
                ShowAddItemWindow(index + 1);
            }
            else
            {
                ShowAddItemWindow(Items.Count); // Default to end if nothing selected
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvTodoList.SelectedItem is TodoItem selectedItem)
            {
                Items.Remove(selectedItem);
            }
        }

        private void ShowAddItemWindow(int insertIndex)
        {
            var addWindow = new AddTodoItemWindow { Owner = this };
            if (addWindow.ShowDialog() == true)
            {
                Items.Insert(insertIndex >= 0 ? insertIndex : Items.Count, new TodoItem { Title = addWindow.NewTitle! });
            }
        }


        // TODO: Implement this drag-and-drop functionality for reordering items in the list.
        // THIS MUST BE DONE ON A CHILD WINDOW OR SIMILAR RENDERING OF THE LIST WITHOUT CHECKBOX OR TEXTBOX CONTROLS.
        private void TodoList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is TodoItem draggedItem)
            {
                if (element is not (CheckBox or TextBox) && e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDrop.DoDragDrop(lvTodoList, draggedItem, DragDropEffects.Move);
                }
            }
        }
        private void TodoList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TodoItem)) is TodoItem droppedItem)
            {
                var target = ((FrameworkElement)e.OriginalSource).DataContext as TodoItem;
                int oldIndex = Items.IndexOf(droppedItem);
                int newIndex = target != null ? Items.IndexOf(target) : Items.Count - 1;

                if (oldIndex != newIndex)
                {
                    Items.Move(oldIndex, newIndex);
                }
            }
        }

        private void btnSetNow_Click(object sender, RoutedEventArgs e)
        {
            if (lvTodoList.SelectedIndex >= 0)
            {
                var selectedItem = Items[lvTodoList.SelectedIndex];
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

public class TodoItem : INotifyPropertyChanged
{
    private string _title = "";
    private bool _complete;
    private DateTime _time;
    private bool _timeEntered;
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
    public bool TimeEntered
    {
        get => _timeEntered;
        set { _timeEntered = value; OnPropertyChanged(); }
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
