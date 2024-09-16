using GGUFLoader.UserPages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary;
namespace GGUFLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeNavi();
            ConfigIO.Initialize();
        }

        void InitializeNavi()
        {
            HomePage_btn.PreviewMouseLeftButtonUp += (_, _) => { MainFrame.Navigate(new HomePage()); };
            ChatsPage_btn.PreviewMouseLeftButtonUp += (_, _) => { MainFrame.Navigate(new ChatPage()); };
            LocalPage_btn.PreviewMouseLeftButtonUp += (_, _) => { MainFrame.Navigate(new LocalModelPage()); };
            OnlinePage_btn.PreviewMouseLeftButtonUp += (_, _) => { MainFrame.Navigate(new OnlineModelPage()); };
            Setting_btn.PreviewMouseLeftButtonUp += (_, _) => { MainFrame.Navigate(new SettingPage()); };
            DownloadTaskPage_btn.PreviewMouseLeftButtonUp += (_, _) => { MainFrame.Navigate(new DownloadTaskPage()); };
        }

        public void NavigateToPage(Page page)
        {
            if (MainFrame != null)
            {
                MainFrame.Navigate(page); 
            }
        }

        public void toOnline()
        {
            HomePage_btn.IsSelected = false;
            OnlinePage_btn.IsSelected = true;
        }
    }
}