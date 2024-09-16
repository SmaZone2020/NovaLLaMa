using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GGUFLoader.UserPages.Tips
{
    /// <summary>
    /// Tip_ViewDevelopers.xaml 的交互逻辑
    /// </summary>
    public partial class Tip_ViewDevelopers : Page
    {
        public Tip_ViewDevelopers()
        {
            InitializeComponent();
        }
        private void WebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://sc.n0v4.site") { UseShellExecute = true });
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("mailto:sm4z0n4t@outlook.com") { UseShellExecute = true });
        }

        private void TelegramButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://t.me/SmaZone") { UseShellExecute = true });
        }
    }
}
