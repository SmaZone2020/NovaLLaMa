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
using Microsoft.Windows.Themes;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            dark_mode.IsOn = ThemeSetting.Dark;
        } 
        private void LoadInstallPath()
        {
            InstallPathTextBlock.Text = ConfigIO.GetInstallPath();
        }

        private void SelectInstallPath_Click(object sender, RoutedEventArgs e)
        {
            if(Directory.Exists(InstallPathTextBlock.Text))
            {
                ConfigIO.SetInstallPath(InstallPathTextBlock.Text);
            }
            else
            {
                MessageBox.Show($"[{InstallPathTextBlock.Text}] does not exist.");
            }
        }

        private void ClearData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string configFilePath = "config.ndbf";

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

        private void dark_mode_Toggled(object sender, RoutedEventArgs e)
        {
            ThemeSetting.Dark = dark_mode.IsOn;
            if(dark_mode.IsOn == true)
            {
                ThemeSetting.SetRequestedTheme("Dark");
            }
            else
            {
                ThemeSetting.SetRequestedTheme("Light");
            }
        }
    }
}
