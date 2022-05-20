using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace NSTeam
{
    /// <summary>
    /// Логика взаимодействия для wpfReport.xaml
    /// </summary>
    public partial class wpfReport : Window
    {
        string reportMsg; //ICollection<>
        public wpfReport(string s)
        {
            InitializeComponent();
            reportMsg = s;
            ListBoxReportMsg.ItemsSource = reportMsg;
        }
    }
}
