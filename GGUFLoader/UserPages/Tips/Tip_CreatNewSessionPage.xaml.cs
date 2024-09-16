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

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// Tip_CreatNewSessionPage.xaml 的交互逻辑
    /// </summary>
    public partial class Tip_CreatNewSessionPage : Page
    {
        Dictionary<string, string> modelList = new Dictionary<string, string>();
        public Tip_CreatNewSessionPage()
        {
            InitializeComponent();
            ID.Text = DateTime.Now.ToString("NCT-yyyyMMddHHmmss");
            foreach (var item in ConfigIO.GetModelList())
            {
                modelList.Add(item.Name, item.FilePath);
                ModelList.Items.Add(item.Name);
            }
            ModelList.SelectedIndex = 0;
        }

        private void AddSession_Click(object sender, RoutedEventArgs e)
        {
            Creating.Name = Name.Text;
            Creating.ID = ID.Text;
            Creating.Model = ModelList.SelectedValue.ToString();
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToPage(new Tips.Tip_CreatNextPage());
        }
    }
}
