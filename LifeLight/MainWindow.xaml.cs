﻿using LifeLight;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Threading;

namespace LifeLight
{
    public partial class MainWindow : Window
    {
        public Dictionary<DateTime, DateLog> Log { get; set; } = [];

        private readonly DispatcherTimer _dateCheckTimer;

        private DateTime _lastCheckedDate;

        private DateTime SelectedDay;

        public MainWindow()
        {
            InitializeComponent();

            MaxHeight = SystemParameters.WorkArea.Height;

            Loaded += (s, e) =>
            {
                double bottomEdge = Top + ActualHeight;
                double workingAreaBottom = SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height;
                if (bottomEdge > workingAreaBottom)
                {
                    double overflow = bottomEdge - workingAreaBottom;
                    Top -= overflow;
                    if (Top < SystemParameters.WorkArea.Top) Top = SystemParameters.WorkArea.Top;
                }
            };

            SizeChanged += (s, e) =>
            {
                double bottomEdge = Top + ActualHeight;
                double workingAreaBottom = SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height;
                if (bottomEdge > workingAreaBottom)
                {
                    double overflow = bottomEdge - workingAreaBottom;
                    Top -= overflow;
                    if (Top < SystemParameters.WorkArea.Top) Top = SystemParameters.WorkArea.Top;
                }
            };

            // Initialize the last checked date to yesterday. This will autopopulate today if needed.
            _lastCheckedDate = DateTime.Today.AddDays(-1);

            // Set up a timer to check for midnight rollover
            _dateCheckTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _dateCheckTimer.Tick += DateCheckTimer_Tick;
            _dateCheckTimer.Start();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo));

            SelectedDay = DateTime.Today;
            AddDayToLog(SelectedDay);
            CalDate.SelectedDate = SelectedDay;
            PopulateDummyData();
            // Update LastCompletedDate for each VariableTodoItem
            foreach (var vn in Log[SelectedDay].VariableNeeds)
            {
                vn.UpdateLastCompletedDate(Log); // Pass the entire Log dictionary
            }
            lvDailyNeeds.ItemsSource = Log[SelectedDay].DailyNeeds;
            lvVariableNeeds.ItemsSource = Log[SelectedDay].VariableNeeds;
            DailyDetailsPanel.DataContext = Log[SelectedDay];

        }

        private void PopulateDummyData()
        {
            // Populate with dummy data for testing
            Log[SelectedDay].VariableNeeds =
            [
                new VariableTodoItem() { Title = "Poot", DaysFrequency = 2 },
                new VariableTodoItem() { Title = "Fart" },
                new VariableTodoItem() { Title = "Shart" }
            ];

            Log[SelectedDay].DailyNeeds =
                [
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
                new DailyTodoItem() { Title = "PM Brush Teeth / Mouthwash" },
            ];
        }

        private void AddDayToLog(DateTime selectedDate)
        {
            DateTime yesterday = selectedDate.AddDays(-1);

            if (!Log.ContainsKey(selectedDate))
            {
                var newDailyNeeds = new ObservableCollection<DailyTodoItem>();
                var newVariableNeeds = new ObservableCollection<VariableTodoItem>();

                if (Log.TryGetValue(yesterday, out var yesterdayLog))
                {
                    foreach (var item in yesterdayLog.DailyNeeds)
                        newDailyNeeds.Add(new DailyTodoItem { Title = item.Title, TimeVisibility = item.TimeVisibility });
                    foreach (var item in yesterdayLog.VariableNeeds)
                        newVariableNeeds.Add(new VariableTodoItem { Title = item.Title, DaysFrequency = item.DaysFrequency });
                }

                Log[selectedDate] = new DateLog
                {
                    Date = selectedDate,
                    DailyNeeds = newDailyNeeds,
                    VariableNeeds = newVariableNeeds
                };
            }

        }

        private void BtnAppTODO_Click(object sender, RoutedEventArgs e)
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

        private void TbTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox tb && tb.DataContext is DailyTodoItem item)
                {
                    TbTime_Validate(tb, item);
                    e.Handled = true; // Prevent further processing (e.g., newline)
                }
            }
        }

        private void TbTime_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.DataContext is DailyTodoItem item)
            {
                TbTime_Validate(tb, item);
            }
        }

        private static void TbTime_Validate(TextBox tb, DailyTodoItem item)
        {
            if (DateTime.TryParse(tb.Text, out DateTime time))
            {
                item.Time = time;
                // Optionally format for display (but keep raw for editing)
                tb.Text = time.ToString("hh:mm tt");
            }
            else
            {
                MessageBox.Show("Please enter a valid time (e.g., 01:30 PM or 13:30).", "Invalid Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb.Text = item.Time.ToString("hh:mm tt"); // Revert to last valid value
            }
        }

        private void CbComplete_Click(object sender, RoutedEventArgs e)
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

        private void AddAboveVariable_Click(object sender, RoutedEventArgs e)
        {
            int index = lvVariableNeeds.SelectedIndex;
            if (index >= 0)
            {
                ShowAddVariableItemWindow(index);
            }
            else
            {
                ShowAddVariableItemWindow(0);
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
                ShowAddDailyItemWindow(Log[SelectedDay].DailyNeeds.Count); // Default to end if nothing selected
            }
        }

        private void AddBelowVariable_Click(object sender, RoutedEventArgs e)
        {
            int index = lvVariableNeeds.SelectedIndex;
            if (index >= 0)
            {
                ShowAddVariableItemWindow(index + 1);
            }
            else
            {
                ShowAddVariableItemWindow(Log[SelectedDay].VariableNeeds.Count); // Default to end if nothing selected
            }
        }

        private void EditDailyItem_Click(object sender, RoutedEventArgs e)
        {
            int index = lvDailyNeeds.SelectedIndex;
            if (index >= 0)
            {
                var editWindow = new EditDailyItemWindow(Log[SelectedDay].DailyNeeds[index]) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    // Update the existing item with new values
                    Log[SelectedDay].DailyNeeds[index].Title = editWindow.NewTitle!;
                    Log[SelectedDay].DailyNeeds[index].TimeVisibility = editWindow.NewTimeVisibility;
                }
            }
        }

        private void EditVariableItem_Click(object sender, RoutedEventArgs e)
        {
            int index = lvVariableNeeds.SelectedIndex;
            if (index >= 0)
            {
                var editWindow = new EditVariableItemWindow(Log[SelectedDay].VariableNeeds[index]) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    //Update the existing item with new values
                    Log[SelectedDay].VariableNeeds[index].Title = editWindow.NewTitle!;
                    Log[SelectedDay].VariableNeeds[index].DaysFrequency = editWindow.NewDaysFrequency;
                    Log[SelectedDay].VariableNeeds[index].DueDateVisibility = editWindow.NewDueDateVisibility;
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
                    Log[SelectedDay].DailyNeeds.Remove(selectedItem);
                }
            }
        }

        private void DeleteVariableItem_Click(object sender, RoutedEventArgs e)
        {
            if (lvVariableNeeds.SelectedItem is VariableTodoItem selectedItem)
            {
                var dialog = new ConfirmDialog(this, "Are you sure you want to delete this item?", "Confirm Deletion");
                if (dialog.ShowDialog() == true)
                {
                    Log[SelectedDay].VariableNeeds.Remove(selectedItem);
                }
            }
        }

        private void SortDailyItems_Click(object sender, RoutedEventArgs e)
        {
            if (Log[SelectedDay].DailyNeeds.Count == 0)
            {
                MessageBox.Show("No items to sort.", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var sortWindow = new SortTodoItemsWindow(new ObservableCollection<DailyTodoItem>(Log[SelectedDay].DailyNeeds)) { Owner = this };
            if (sortWindow.ShowDialog() == true)
            {
                Log[SelectedDay].DailyNeeds.Clear();
                foreach (var item in sortWindow.SortedDailyItems!)
                {
                    Log[SelectedDay].DailyNeeds.Add(item);
                }
            }
        }

        private void SortVariableItems_Click(object sender, RoutedEventArgs e)
        {
            if (Log[SelectedDay].VariableNeeds.Count == 0)
            {
                MessageBox.Show("No items to sort.", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var sortWindow = new SortTodoItemsWindow(new ObservableCollection<VariableTodoItem>(Log[SelectedDay].VariableNeeds)) { Owner = this };
            if (sortWindow.ShowDialog() == true)
            {
                Log[SelectedDay].VariableNeeds.Clear();
                foreach (var item in sortWindow.SortedVariableItems!)
                {
                    Log[SelectedDay].VariableNeeds.Add(item);
                }
            }
        }

        private void ShowAddDailyItemWindow(int insertIndex)
        {
            var addWindow = new AddDailyItemWindow { Owner = this };
            if (addWindow.ShowDialog() == true)
            {
                Log[SelectedDay].DailyNeeds.Insert(insertIndex >= 0 ? insertIndex : Log[SelectedDay].DailyNeeds.Count,
                    new DailyTodoItem
                    {
                        Title = addWindow.NewTitle!,
                        TimeVisibility = addWindow.NewTimeVisibility
                    });
            }
        }

        private void ShowAddVariableItemWindow(int insertIndex)
        {
            var addWindow = new AddVariableItemWindow { Owner = this };
            if (addWindow.ShowDialog() == true)
            {
                Log[SelectedDay].VariableNeeds.Insert(insertIndex >= 0 ? insertIndex : Log[SelectedDay].VariableNeeds.Count,
                    new VariableTodoItem
                    {
                        Title = addWindow.NewTitle!,
                        DueDateVisibility = addWindow.NewDueDateVisibility,
                        DaysFrequency = addWindow.NewDaysFrequency
                    });
            }
        }

        private void BtnSetNow_Click(object sender, RoutedEventArgs e)
        {
            if (lvDailyNeeds.SelectedIndex >= 0)
            {
                var selectedItem = Log[SelectedDay].DailyNeeds[lvDailyNeeds.SelectedIndex];
                selectedItem.Time = DateTime.Now;
                //MessageBox.Show($"Time set to: {selectedItem.Time.ToString("hh:mm tt")}");
            }
        }

        private void DateCheckTimer_Tick(object? sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            if (today > _lastCheckedDate) // Only update if the day has changed
            {
                CalDate.DisplayDateEnd = today; // Updates calendar range
                _lastCheckedDate = today; // Update the last checked date
                AddDayToLog(today); // Just prep Log[today], no UI change
                SetColorsForDate();
            }
        }

        private void SetColorsForDate()
        {
            if (CalDate.SelectedDate.HasValue)
            {
                if (CalDate.SelectedDate.Value.Date < DateTime.Today)
                {
                    Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)); // Light gray-blue
                    RootDockPanel.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                    CalDate.Background = new SolidColorBrush(Colors.PaleGoldenrod);
                    CalDate.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 153, 0));
                    CalDate.BorderThickness = new Thickness(2);
                    CalDate.Margin = new Thickness(10, 12, 10, 10); // Adjusted from 13 to 12 for thicker border
                    TbkDailyRating.Margin = new Thickness(10, 9, 0, 0); // Adjusted from 10 to 9 for thicker border
                }
                else
                {
                    Background = new SolidColorBrush(Colors.White);
                    RootDockPanel.Background = new SolidColorBrush(Colors.White);
                    CalDate.Background = new SolidColorBrush(Colors.White);
                    CalDate.BorderBrush = new SolidColorBrush(Colors.Gray);
                    CalDate.BorderThickness = new Thickness(1);
                    CalDate.Margin = new Thickness(10, 13, 10, 10); // Original margin
                    TbkDailyRating.Margin = new Thickness(10, 10, 0, 0); // Original margin
                }
            }
        }

        private void CalDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!CalDate.SelectedDate.HasValue) return;
            
            SelectedDay = CalDate.SelectedDate.Value;
            
            AddDayToLog(SelectedDay);
            
            SetColorsForDate();
   
            // Update LastCompletedDate for each VariableTodoItem
            foreach (var vn in Log[SelectedDay].VariableNeeds)
            {
                vn.UpdateLastCompletedDate(Log); // Pass the entire Log dictionary
            }

            // Style the TextBlocks manually
            foreach (var item in lvVariableNeeds.Items)
            {
                var container = lvVariableNeeds.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container != null)
                {
                    var textBlock = container.FindName("tblkDueDate") as TextBlock;
                    if (textBlock != null && item is VariableTodoItem vti && vti.DueDate.HasValue)
                    {
                        TimeSpan difference = vti.DueDate.Value - SelectedDay;

                        if (difference.Days == 0)
                        {
                            textBlock.Text = "Due Today";
                            textBlock.Foreground = Brushes.Black;
                            textBlock.FontWeight = FontWeights.Bold;
                        }
                        else
                        {
                            textBlock.Text = $"Due {vti.DueDate.Value:ddd MMM dd}";
                            textBlock.Foreground = difference.Days < 0 ? Brushes.Red : Brushes.Black;
                            textBlock.FontWeight = difference.Days < 0 ? FontWeights.Bold : FontWeights.Normal;
                        }
                    }
                }
            }

            DailyDetailsPanel.DataContext = Log[SelectedDay];
            lvDailyNeeds.ItemsSource = Log[SelectedDay].DailyNeeds;
            lvVariableNeeds.ItemsSource = Log[SelectedDay].VariableNeeds;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export clicked!");
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

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class VariableTodoItem : INotifyPropertyChanged
    {
        private string _title = "";
        private bool _complete;
        private int _daysFrequency = 0;
        private string _comment = "";
        private Visibility _dueDateVisibility = Visibility.Hidden;
        //todo: replace ai code. change DueDate to a regular get, set property.
        //If lastCompletedDate == null then duedate = selectedDay
        //else, duedate = lastCompletedDate + frequency
        //Move UI updating out of calendar handler and place a call to it there
        //Call the UI update when item is checked.
        //Only update the checked item in that case (maybe with a unique UI updating method)
        //checking a past item will have to update the lastCompletedDate for all future items until a new completed date is found
        //THIS ^ ^ ^ IS WHY I LIKE MY ORIGINAL IDEA BETTER
        //NO KEEPING THESE ADDITIONAL VARIABLES TO JUGGLE AND UPDATE. THAT WAS AN    A R T I F I C I A L    "INTELLIGENCE" IDEA.
        //JUST DO THE WORK EVERY TIME THE DAY CHANGES AND STOP KEEPING EXTRA VARIABLES TO MANAGE
        //ALL YOU HAVE TO DO IS LOOK BACK IN TIME FOR THE SAME ENTRY
        //IF IT DOES NOT EXIST ON A DAY, THEN EXIT FOR.
        //IF YOU FIND A COMPLETION, EXIT FOR.
        //IF YOU DON'T FIND IT, THEN IT IS DUE TODAY.
        //IF THERE IS NO DAYSFREQUENCY, THEN YOU DON'T EVEN HAVE TO LOOK.
        //IF THE BOX IS TICKED, THEN IT DOES NOT NEED TO JUGGLE SHIT IN THE FUTURE. IT IS JUST COMPLETE.
        //  IT WILL ONLY NEED TO UPDATE THE UI FOR THIS ITEM AND NOTHING ELSE IS CHANGED.
        //  MY IDEA WAS BETTER FROM THE START. I FELL VICTIM TO MY OWN IGNORANT LAZINESS IN LETTING THE AI GIVE ME CODE.
        //  DON'T DISCOUNT MY OWN INTUITION, ESPECIALLY - ESPECIALLY FOR DATA STRUCTURE LIKE THIS.
        //  IT'S OKAY TO ASK FOR INPUT BUT ...
        //REMEMBER: IF IT FEELS TOO COMPLICATED, THAT MEANS IT IS.

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
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }
        public int DaysFrequency
        {
            get => _daysFrequency;
            set
            {
                _daysFrequency = value;
                DueDateVisibility = value > 0 ? Visibility.Visible : Visibility.Hidden; // Show due date if frequency is set
                OnPropertyChanged();
            }
        }
        public Visibility DueDateVisibility
        {
            get => _dueDateVisibility;
            set { _dueDateVisibility = value; OnPropertyChanged(); }
        }


        // todo: alter this Method to find and set last completion date from the log
        public void UpdateLastCompletedDate(Dictionary<DateTime, DateLog> log)
        {
            if (DaysFrequency <= 0) return;

            DateTime today = DateTime.Today;
            DateTime checkDate = today;

            while (checkDate >= log.Keys.Min())
            {
                if (log.TryGetValue(checkDate, out DateLog? dateLog))
                {
                    var completedItem = dateLog.VariableNeeds
                        .FirstOrDefault(v => v.Title == this.Title && v.Complete);

                    if (completedItem != null)
                    {
                        //todo remember this is just a bs placeholder
                        var pootLastCompletedDate = checkDate;
                        return;
                    }
                }
                checkDate = checkDate.AddDays(-1);
            }
            DateTime tootLastCompletedDate ; // No completion found
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class DateLog : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public ObservableCollection<DailyTodoItem> DailyNeeds { get; set; } = [];
        public ObservableCollection<VariableTodoItem> VariableNeeds { get; set; } = [];
        private int _rating = 5;
        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

        private string _notes = "";
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // No need for two-way binding here
        }
    }

}