using System;
using System.Collections;
using System.Collections.Generic;
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
        public AppTODOWindow()
        {
            InitializeComponent();

            tbAppTODO.Text += "Build out edit menu.";
            tbAppTODO.Text += "\n\nAdd ListView for optional items like bath. Add Frequency property in backend (in Add / Edit window). ";
            tbAppTODO.Text += "\n\nAdd Edit context menu item to change an entry";

            tbAppTODO.Text += "\n\nAdd mood rating trackbar";

            tbAppTODO.Text += "\n\nAdd daily comments textbox";

            tbAppTODO.Text += "\n\nAdd Reports button to show reports window with checkboxes for each item to report, ";
            tbAppTODO.Text += "start date calendar, end date calendar, report button on that window.";



        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
                Close();
        }

    }
}
