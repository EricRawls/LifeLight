using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeLight
{
    public partial class SortTodoItemsWindow : Window
    {
        public ObservableCollection<DailyTodoItem>? DailyItems { get; private set; }
        public ObservableCollection<DailyTodoItem>? SortedDailyItems => DailyItems; // For binding

        public ObservableCollection<VariableTodoItem>? VariableItems { get; private set; }
        public ObservableCollection<VariableTodoItem>? SortedVariableItems => VariableItems; // For binding

        public SortTodoItemsWindow(ObservableCollection<DailyTodoItem> sourceItems)
        {
            InitializeComponent();
            DailyItems = new ObservableCollection<DailyTodoItem>(sourceItems);
            lbSortList.ItemsSource = SortedDailyItems;
        }

        public SortTodoItemsWindow(ObservableCollection<VariableTodoItem> sourceItems)
        {
            InitializeComponent();
            VariableItems = new ObservableCollection<VariableTodoItem>(sourceItems);
            lbSortList.ItemsSource = SortedVariableItems;
        }

        private void lbSortList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is DailyTodoItem draggedItem)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // Ensure the ListBox is the drag source
                    DragDrop.DoDragDrop(lbSortList, new DataObject(typeof(DailyTodoItem), draggedItem), DragDropEffects.Move);
                    e.Handled = true; // Prevent other handlers from interfering
                }
            }
            else if (e.OriginalSource is FrameworkElement element2 && element2.DataContext is VariableTodoItem draggedItem2)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // Ensure the ListBox is the drag source
                    DragDrop.DoDragDrop(lbSortList, new DataObject(typeof(VariableTodoItem), draggedItem2), DragDropEffects.Move);
                    e.Handled = true; // Prevent other handlers from interfering
                }
            }
        }

        private void lbSortList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(DailyTodoItem)) is DailyTodoItem droppedDailyItem)
            {
                var target = e.OriginalSource as FrameworkElement;
                while (target != null && !(target.DataContext is DailyTodoItem))
                {
                    target = target.Parent as FrameworkElement;
                }
                var targetItem = target?.DataContext as DailyTodoItem;
                int oldIndex = DailyItems!.IndexOf(droppedDailyItem);
                int newIndex = targetItem != null ? DailyItems.IndexOf(targetItem) : DailyItems.Count - 1;

                if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0 && newIndex < DailyItems.Count)
                {
                    DailyItems.Move(oldIndex, newIndex);
                }
                e.Handled = true; // Mark the event as handled                
            }
            else if (e.Data.GetData(typeof(VariableTodoItem)) is VariableTodoItem droppedVariableItem)
            {
                var target = e.OriginalSource as FrameworkElement;
                while (target != null && !(target.DataContext is VariableTodoItem))
                {
                    target = target.Parent as FrameworkElement;
                }
                var targetItem = target?.DataContext as VariableTodoItem;
                int oldIndex = VariableItems!.IndexOf(droppedVariableItem);
                int newIndex = targetItem != null ? VariableItems.IndexOf(targetItem) : VariableItems.Count - 1;
                if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0 && newIndex < VariableItems.Count)
                {
                    VariableItems.Move(oldIndex, newIndex);
                }
                e.Handled = true; // Mark the event as handled                
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}