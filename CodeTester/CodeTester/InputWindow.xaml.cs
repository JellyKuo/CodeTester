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
    /// InputWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow(string Name, string DefaultValue = "")
        {
            InitializeComponent();
            Title = Name;
            txtName.Text = Name;
            txtValue.Text = DefaultValue;
        }

        public int Value;

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnConf_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out Value))
            {
                MessageBox.Show("Input should be a integer.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
