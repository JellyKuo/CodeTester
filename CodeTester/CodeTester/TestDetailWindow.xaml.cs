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
using System.Windows.Shapes;

namespace NPSC
{
    /// <summary>
    /// TestDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class TestDetailWindow : Window
    {
        public TestDetailWindow(Test test)
        {
            InitializeComponent();
            DataContext = test;
        }
    }
}
