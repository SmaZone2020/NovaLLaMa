using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class Runtimes
    {
        public static ObservableCollection<DownloadTask> DownloadTasks { get; } = new ObservableCollection<DownloadTask>();
        public class RunModel
        {
            public static bool IsRuning { get; set; } = false;
            public static string ModelName { get; set; }
            public static DateTime StartTime { get; set; }
            public static List<ChatHistory> ChatHistory { get; set; }
        }
    }

    public class Creating
    {
        public static string Name { get; set; }
        public static string Model { get; set; }
        public static string AssistantName { get;set; }
        public static string ID { get; set; }
    }

    public class DownloadTask : INotifyPropertyChanged
    {
        private long _downloadedSizeMB;
        private double _currentSpeedMB;
        private string _estimatedTimeRemaining;

        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string DownloadPath { get; set; }
        public long FileSizeMB { get; set; }

        public long DownloadedSizeMB
        {
            get => _downloadedSizeMB;
            set
            {
                _downloadedSizeMB = value;
                OnPropertyChanged(nameof(DownloadedSizeMB));
            }
        }

        public double CurrentSpeedMB
        {
            get => _currentSpeedMB;
            set
            {
                _currentSpeedMB = value;
                OnPropertyChanged(nameof(CurrentSpeedMB));
            }
        }

        public string EstimatedTimeRemaining
        {
            get => _estimatedTimeRemaining;
            set
            {
                _estimatedTimeRemaining = value;
                OnPropertyChanged(nameof(EstimatedTimeRemaining));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
