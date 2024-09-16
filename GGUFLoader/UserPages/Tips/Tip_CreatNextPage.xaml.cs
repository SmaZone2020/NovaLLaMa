using ClassLibrary;
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

namespace GGUFLoader.UserPages.Tips
{
    /// <summary>
    /// Tip_CreatNextPage.xaml 的交互逻辑
    /// </summary>
    public partial class Tip_CreatNextPage : Page
    {

        public Tip_CreatNextPage()
        {
            InitializeComponent();
        }

        private void AddSession_Click(object sender, RoutedEventArgs e)
        {
            var setting = "[]";
            if(!String.IsNullOrEmpty(Setting.Text) && Setting.Text.Length > 1)
                setting = Setting.Text;

            if(ASName.Text.Length > 0)
            {
                Creating.AssistantName = ASName.Text;
            }

            ConfigIO.AddSession(Creating.Name,
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                Creating.Model,
                                Creating.ID,
                                Creating.AssistantName,
                                setting);
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("Session Created.");
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToPage(new ChatPage());
        }
    }
}
