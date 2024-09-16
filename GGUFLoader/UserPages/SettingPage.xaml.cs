using Microsoft.Win32;
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
using System.IO;
using ClassLibrary;

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        private string installPath;
        public SettingPage()
        {
            InitializeComponent();
        }        // 加载安装目录（模拟从配置文件读取路径）
        private void LoadInstallPath()
        {
            installPath = ConfigIO.GetInstallPath();
            UpdateInstallPathDisplay();
        }

        private void UpdateInstallPathDisplay()
        {
            InstallPathTextBlock.Text = !string.IsNullOrEmpty(installPath) ? installPath : "none";
        }

        private void SelectInstallPath_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string configFilePath = Path.Combine(installPath, "config.ndbf");

                if (File.Exists(configFilePath))
                {
                    File.Delete(configFilePath);
                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("Data has been cleared", "Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"Error clearing data:{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
