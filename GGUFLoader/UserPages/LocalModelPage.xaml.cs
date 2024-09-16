using ClassLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using LLama.Common;
using LLama;
using LLama.Transformers;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;
namespace GGUFLoader.UserPages
{
    /// <summary>
    /// LocalModelPage.xaml 的交互逻辑
    /// </summary>
    public partial class LocalModelPage : Page
    {
        public ObservableCollection<Model> Models { get; set; }
        public LocalModelPage()
        {
            InitializeComponent();
            ConfigIO.Initialize();
            Models = new ObservableCollection<Model>(ConfigIO.GetModelList());
            ModelListView.ItemsSource = Models;
        }
        private void AddModelButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "GGUF Files (*.gguf)|*.gguf",
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
                Models.Add(newModel);

            }
        }
        private void DeleteModelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelListView.SelectedItem is Model selectedModel)
            {
                ConfigIO.DeleteModel(selectedModel.ID);
                Models.Remove(selectedModel);
            }
        }
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelListView.SelectedItem is Model selectedModel)
            {
                string folderPath = Path.GetDirectoryName(selectedModel.FilePath);
                if (Directory.Exists(folderPath))
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else
                {
                    MessageBox.Show("文件夹不存在！");
                }
            }
        }

        private void ModelListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isModelSelected = ModelListView.SelectedItem != null;
            DeleteModelButton.IsEnabled = isModelSelected;
            OpenFolderButton.IsEnabled = isModelSelected;
        }

        private void SearchBox_TextChanged(iNKORE.UI.WPF.Modern.Controls.AutoSuggestBox sender, iNKORE.UI.WPF.Modern.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            string searchText = sender.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                ModelListView.ItemsSource = Models;
            }
            else
            {
                var filteredModels = Models.Where(model => model.Name.ToLower().Contains(searchText)).ToList();
                ModelListView.ItemsSource = filteredModels;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigIO.GetModelList().Count < 1)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.NavigateToPage(new Tip_LoadAndDownload());
            }
        }
    }
}
