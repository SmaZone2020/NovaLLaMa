using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page
    {
        public ChatPage()
        {
            InitializeComponent();
            InitializeModelList();
            InitializeSession();
            AddDialogTextAsync("ChatGPT","你好，有需要帮助的吗。",Brushes.White);
            AddDialogTextAsync("User", "你叫什么名字", Brushes.LightBlue);
        }
        public List<Session> sessions;
        public Session nowSession;
        private void InitializeSession()
        {
            sessions = ConfigIO.GetAllSessions();
            foreach (var item in sessions)
            {
                var session = new iNKORE.UI.WPF.Modern.Controls.NavigationViewItem
                {
                    Content = item.Name,
                    Tag = item.ID,
                };

                session.Icon = new iNKORE.UI.WPF.Modern.Controls.FontIcon
                {
                    Glyph = "\ue70b"
                };

                ChatSessions.MenuItems.Add(session);
            }

            if (ChatSessions.MenuItems.Count > 0)
            {
                nowSession = sessions.FirstOrDefault<Session>();
                ChatSessions.SelectedItem = ChatSessions.MenuItems[0];
                ModelList.Text = nowSession.ModelName;
            }
            

            /*
                         var homePageItem = new iNKORE.UI.WPF.Modern.Controls.NavigationViewItem
                        {
                            Content = "Session1",
                            Tag = "chat1",
                        };

                        homePageItem.Icon = new iNKORE.UI.WPF.Modern.Controls.FontIcon
                        {
                            Glyph = "\ue70b"
                        };

                        ChatSessions.MenuItems.Add(homePageItem);
             */
        }

        public async Task AddDialogTextAsync(string role, string message, Brush textColor)
        {
            TextBlock dialogTextBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new System.Windows.Thickness(0, 10, 0, 10)
            };

            Run speakerRun = new Run($"{role}: ") { Foreground = Brushes.Gray, FontWeight = FontWeights.Bold };
            Run messageRun = new Run();

            dialogTextBlock.Inlines.Add(speakerRun);
            dialogTextBlock.Inlines.Add(messageRun);

            DialogStackPanel.Children.Add(dialogTextBlock);
            for (int i = 0; i <= message.Length; i++)
            {
                messageRun.Text = message.Substring(0, i);
                messageRun.Foreground = textColor;
                await Task.Delay(50); // 控制逐字显示的速度，50ms/字
            }
        }

        Dictionary<string,string> modelList = new Dictionary<string,string>();
        public void InitializeModelList()
        {
            foreach (var item in ConfigIO.GetModelList())
            {
                modelList.Add(item.Name, item.FilePath);
                ModelList.Items.Add(item.Name);
            }
        }

        private void newSession_Click(object sender, RoutedEventArgs e)
        {
            //ConfigIO.AddSession();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;

            if (ConfigIO.GetModelList().Count < 1)
                mainWindow.NavigateToPage(new Tip_LoadAndDownload());

            else if (ConfigIO.GetAllSessions().Count < 1)
                mainWindow.NavigateToPage(new Tip_CreatNewSessionPage());
            
        }

        private void delSession_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ChatSessions.SelectedItem as iNKORE.UI.WPF.Modern.Controls.NavigationViewItem;

            if (selectedItem != null)
            {
                var selectedTag = selectedItem.Tag?.ToString();
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"Selected Tag: {selectedTag}");
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        public void SendMessage()
        {
            AddDialogTextAsync("User", input_Message.Text, Brushes.LightBlue);
            input_Message.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
    }
}
