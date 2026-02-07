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

namespace YDSkyrimTools
{
    /// <summary>
    /// Interaction logic for AboutAuthor.xaml
    /// </summary>
    public partial class AboutAuthor : Window
    {
        public AboutAuthor()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentVersion.Content = string.Format("SkyrimTools Version : {0}", DeFine.CurrentVersion);
        }
    }
}
