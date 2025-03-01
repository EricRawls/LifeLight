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

            StrAppToDo += "Context menu methods for Variable Items";
            StrAppToDo += "\n\nHelper windows for Variable Items.";
            StrAppToDo += "\n\nDates dictionary";
            StrAppToDo += "\n\nAFTER ALL data structure is built, THEN json storage so I don't have to save notes this way anymore.";
            StrAppToDo += "\n\nReports button to show reports window with checkboxes for each item to report, ";
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
