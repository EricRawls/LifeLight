using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LifeLight
{
    public partial class SortTodoItemsWindow : Window
    {
        public ObservableCollection<TodoItem> Items { get; private set; }
        public ObservableCollection<TodoItem> SortedItems => Items; // For binding

        public SortTodoItemsWindow(ObservableCollection<TodoItem> sourceItems)
        {
            InitializeComponent();
            Items = new ObservableCollection<TodoItem>(sourceItems);
            lbSortList.ItemsSource = SortedItems;
        }

        private void lbSortList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is TodoItem draggedItem)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    // Ensure the ListBox is the drag source
                    DragDrop.DoDragDrop(lbSortList, new DataObject(typeof(TodoItem), draggedItem), DragDropEffects.Move);
                    e.Handled = true; // Prevent other handlers from interfering
                }
            }
        }

        private void lbSortList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TodoItem)) is TodoItem droppedItem)
            {
                var target = e.OriginalSource as FrameworkElement;
                while (target != null && !(target.DataContext is TodoItem))
                {
                    target = target.Parent as FrameworkElement;
                }
                var targetItem = target?.DataContext as TodoItem;
                int oldIndex = Items.IndexOf(droppedItem);
                int newIndex = targetItem != null ? Items.IndexOf(targetItem) : Items.Count - 1;

                if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0 && newIndex < Items.Count)
                {
                    Items.Move(oldIndex, newIndex);
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