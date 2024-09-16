using ClassLibrary;
using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// OnlineModelPage.xaml 的交互逻辑
    /// </summary>
    public partial class OnlineModelPage : System.Windows.Controls.Page
    {
        private List<ModelBrand> brands;
        public OnlineModelPage()
        {
            InitializeComponent();
            LoadBrandsAsync();
        }
        private async void LoadBrandsAsync()
        {
            try
            {
                brands = await ModelService.GetAllBrandsAsync();
                BrandComboBox.ItemsSource = brands;
                BrandComboBox.DisplayMemberPath = "Name";
                BrandComboBox.SelectedValuePath = "Url";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load brands: {ex.Message}");
            }
        }

        private async void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandComboBox.SelectedValue is string selectedUrl)
            {
                try
                {
                    var modelDetails = await ModelService.GetModelDetailsAsync(selectedUrl);
                    ModelListView.ItemsSource = modelDetails;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load model details: {ex.Message}");
                }
            }
        }

        private async void DownloadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ModelListView.SelectedItem is ModelDetail selectedModel)
            {
                try
                {
                    string installPath = ConfigIO.GetInstallPath();
                    string filePath = System.IO.Path.Combine(installPath, selectedModel.Filename);

                    var downloadTask = new DownloadTask
                    {
                        FileName = selectedModel.Filename,
                        FileUrl = selectedModel.Download_Url,
                        DownloadPath = filePath,
                        FileSizeMB = 0,
                        DownloadedSizeMB = 0,
                        CurrentSpeedMB = 0,
                        EstimatedTimeRemaining = "Calculating..."
                    };

                    Runtimes.DownloadTasks.Add(downloadTask);

                    MessageBox.Show($"下载任务已创建.");

                    await DownloadTaskPage.DownloadFileAsync(downloadTask);

                    downloadTask.DownloadedSizeMB = downloadTask.FileSizeMB; 
                    downloadTask.EstimatedTimeRemaining = "Completed";
                    ConfigIO.AddModel(Path.GetFileName(filePath),
                        0,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        filePath,
                        "预设模型");
                    MessageBox.Show($"File downloaded successfully and saved to {filePath}");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to download the file: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a model to download.");
            }
        }


    }
}
