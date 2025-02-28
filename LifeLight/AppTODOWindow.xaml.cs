using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using static System.Net.Mime.MediaTypeNames;

namespace LifeLight
{
    /// <summary>
    /// Interaction logic for ToDoWindow.xaml
    /// </summary>
    public partial class AppTODOWindow : Window
    {
        private string _strAppToDo = "";

        public string StrAppToDo
        {
            get { return _strAppToDo; }
            set
            {
                _strAppToDo = value;
                OnPropertyChanged(nameof(StrAppToDo));
            }
        }

        public AppTODOWindow()
        {
            InitializeComponent();
            DataContext = this; // Set the DataContext to this window

            StrAppToDo += "Build out edit menu.";
            StrAppToDo += "\n\nAdd ListView for optional daily items. Add Frequency property in backend (and in Add / Edit window).";
            StrAppToDo += "\nItems will show label indicating previous instance and next due based on frequency.";
            StrAppToDo += "\n\nAdd Edit context menu item to change an entry";
            StrAppToDo += "\n\nAdd mood rating trackbar";
            StrAppToDo += "\n\nAdd daily comments textbox";
            StrAppToDo += "\n\nAdd Reports button to show reports window with checkboxes for each item to report, ";
            StrAppToDo += "start date calendar, end date calendar, report button on that window.";
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged = null!;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
                Close();
        }

    }
}
