using ClassLibrary;
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
using System.Collections.ObjectModel;

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// Tip_LoadAndDownload.xaml 的交互逻辑
    /// </summary>
    public partial class Tip_LoadAndDownload : Page
    {
        public Tip_LoadAndDownload()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            Page targetPage = new OnlineModelPage();
            mainWindow.NavigateToPage(targetPage);
            mainWindow.toOnline();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var Models = new ObservableCollection<Model>(ConfigIO.GetModelList());
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "GGUF Files (*.gguf)|*.gguf", // 只允许选择gguf文件
                Title = "选择GGUF模型文件"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                string modelName = Path.GetFileNameWithoutExtension(filePath);
                int param = 8;
                string addDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string setting = "模型预设信息";

                var newModel = new Model
                {
                    ID = Models.Count + 1,
                    Name = modelName,
                    Param = param,
                    AddDate = addDate,
                    FilePath = filePath,
                    Setting = setting
                };
                ConfigIO.AddModel(newModel.Name, newModel.Param, newModel.AddDate, newModel.FilePath, newModel.Setting);
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.NavigateToPage(new LocalModelPage());
            }
        }
    }
}
