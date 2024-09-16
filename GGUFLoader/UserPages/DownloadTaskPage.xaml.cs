using ClassLibrary;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Controls;

namespace GGUFLoader.UserPages
{
    public partial class DownloadTaskPage : Page
    {
        public DownloadTaskPage()
        {
            InitializeComponent();
            DownloadTasksListView.ItemsSource = Runtimes.DownloadTasks;
            foreach (var task in Runtimes.DownloadTasks)
            {
                Debug.WriteLine($"{task.FileName},{task.DownloadedSizeMB}/{task.FileSizeMB},{task.FileUrl}");
            }

        }
        public static async Task DownloadFileAsync(DownloadTask task)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (var response = await httpClient.GetAsync(task.FileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    long totalBytes = response.Content.Headers.ContentLength ?? 0;
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(task.DownloadPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        byte[] buffer = new byte[8192];
                        long totalBytesRead = 0;
                        int bytesRead;
                        DateTime startTime = DateTime.Now;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            task.DownloadedSizeMB = totalBytesRead / (1024 * 1024);
                            task.FileSizeMB = totalBytes / (1024 * 1024);

                            // 更新下载速度和预计剩余时间
                            TimeSpan elapsedTime = DateTime.Now - startTime;
                            double speedMB = (elapsedTime.TotalSeconds > 0) ? (task.DownloadedSizeMB / elapsedTime.TotalSeconds) : 0;
                            task.CurrentSpeedMB = speedMB;

                            // 防止 speedMB 为零时的计算溢出
                            double estimatedTimeSeconds = (speedMB > 0) ? ((totalBytes - totalBytesRead) / (speedMB * 1024 * 1024)) : double.MaxValue;
                            task.EstimatedTimeRemaining = (estimatedTimeSeconds < double.MaxValue)
                                ? TimeSpan.FromSeconds(estimatedTimeSeconds).ToString(@"hh\:mm\:ss")
                                : "Calculating...";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                task.EstimatedTimeRemaining = "Error";
                throw;
            }
        }

    }
}
