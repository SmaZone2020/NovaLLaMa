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
using LLama;
using LLama.Common;
using System.Data.Entity.Infrastructure;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;
using Newtonsoft.Json;
using System.Windows.Media.Animation;

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page
    {
        Brush chatColor = Brushes.White;
        Brush userColor = Brushes.LightBlue;
        Brush systemColor = Brushes.OrangeRed;

        public List<Session> ?sessions;
        public Session ?nowSession;

        bool LoadingHistory = true;

        private InteractiveExecutor ?_executor;
        private LLama.Common.ChatHistory ?_chatHistory;
        public ChatPage()
        {
            if(ThemeSetting.GetRequestedTheme() == "Light")
            {
                chatColor = Brushes.Gray;
                userColor = Brushes.DarkBlue;
            }

            InitializeComponent();
            InitializeSession();
        }

        private void InitializeSession()
        {
            LoadingHistory = true;
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
            }
            
        }

        public async Task InitializeModelAsync()
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            LoadingHistory = true;
            try
            {
                foreach (var item in ConfigIO.GetModelList())
                {
                    if (item.Name == nowSession.ModelName)
                    {
                        var parameters = new ModelParams(item.FilePath)
                        {
                            ContextSize = 2048,
                            GpuLayerCount = 6
                        };

                        // 异步加载模型
                        await Task.Run(() =>
                        {
                            var model = LLamaWeights.LoadFromFile(parameters);
                            var context = model.CreateContext(parameters);
                            _executor = new InteractiveExecutor(context);
                        });

                        _chatHistory = new LLama.Common.ChatHistory();
                        if (nowSession.Chats != null && nowSession.Chats.Length > 0)
                        {
                            foreach (var chat in nowSession.Chats)
                            {
                                AuthorRole role = MapRole(chat.role);
                                _chatHistory.AddMessage(role, chat.content);
                                if (role == AuthorRole.System)
                                    await AddDialogTextAsync("System", chat.content, systemColor,true);
                                else if (role == AuthorRole.Assistant)
                                    await AddDialogTextAsync("Assistant", chat.content, chatColor, true);
                                else if (role == AuthorRole.User)
                                    await AddDialogTextAsync("User", chat.content, userColor, true);
                            }
                        }
                        MessageBox.Show($"Found Model {item.Name}");
                        LoadingHistory = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载模型失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        public async Task AddDialogTextAsync(string role, string message, Brush textColor, bool nodelay = false)
        {
            TextBlock dialogTextBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new System.Windows.Thickness(0, 10, 0, 10)
            };

            if (role.ToLower() == "assistant")
                role = nowSession.AssistantName;

            Run speakerRun = new Run($"{role}: ") { Foreground = Brushes.Gray, FontWeight = FontWeights.Bold };
            Run messageRun = new Run();

            dialogTextBlock.Inlines.Add(speakerRun);
            dialogTextBlock.Inlines.Add(messageRun);

            DialogStackPanel.Children.Add(dialogTextBlock);

            if (nodelay)
            {
                messageRun.Text = message;
                messageRun.Foreground = textColor;
            }
            else
            {
                for (int i = 0; i <= message.Length; i++)
                {
                    messageRun.Text = message.Substring(0, i);
                    messageRun.Foreground = textColor;
                    await Task.Delay(20);
                }
            }

            if (!LoadingHistory)
            {
                if (role.ToLower() != "system")
                {
                    var sessionList = nowSession.Chats.ToList();
                    sessionList.Add(new ClassLibrary.ChatHistory { role = role, content = message });
                    nowSession.Chats = sessionList.ToArray();

                    var json = JsonConvert.SerializeObject(sessionList, Formatting.Indented);

                    ConfigIO.UpdateSessionContent(nowSession.ID, json);
                    UpdateText();
                }
            }
            DialogScrollViewer.ScrollToEnd();
        }

        private void newSession_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToPage(new Tip_CreatNewSessionPage());

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingHistory = true;
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
                if(iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                    $"The selected conversations will be deleted: {selectedTag}\n" +
                    $"Are you sure you want to delete?\n" +
                    $"This operation is irreversible.", 
                    "Alert", 
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ConfigIO.DeleteSession(selectedTag);
                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show($"Deleted conversation with ID {selectedTag}");
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.NavigateToPage(new ChatPage());
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        public async void SendMessage()
        {
            input_Message.IsEnabled = false;

            string userInput = input_Message.Text;
            await AddDialogTextAsync("User", userInput, userColor);

            _chatHistory.AddMessage(AuthorRole.User, userInput);
            input_Message.Text = "Responding...";
            string chatHistoryString = ConvertChatHistoryToString();
            InferenceParams inferenceParams = new InferenceParams()
            {
                MaxTokens = 256,
                AntiPrompts = new List<string> { "User:" }
            };

            string assistantResponse = "";
            var result = _executor.InferAsync(chatHistoryString, inferenceParams);

            await foreach (var text in result)
            {
                assistantResponse += text;
            }

            if (assistantResponse.Length > 5)
            {
                assistantResponse = assistantResponse.Substring(0, assistantResponse.Length - 5).Replace("\"","'");
            }

            _chatHistory.AddMessage(AuthorRole.Assistant, assistantResponse);
            await AddDialogTextAsync(nowSession.AssistantName, assistantResponse, chatColor);
            input_Message.IsEnabled = true;
            input_Message.Text = "";

        }

        private string ConvertChatHistoryToString()
        {
            StringBuilder chatHistoryBuilder = new StringBuilder();

            foreach (var message in _chatHistory.Messages)
            {
                string role = message.AuthorRole == AuthorRole.User ? "User" :
                              message.AuthorRole == AuthorRole.Assistant ? "Assistant" : "System";
                chatHistoryBuilder.AppendLine($"{role}: {message.Content}");
            }

            return chatHistoryBuilder.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e) => SendMessage();

        private AuthorRole MapRole(string roleString)
        {
            // 角色映射
            if (roleString.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                return AuthorRole.User;
            }
            else if (roleString.Equals("assistant", StringComparison.OrdinalIgnoreCase))
            {
                return AuthorRole.Assistant;
            }
            else if (roleString.Equals("system", StringComparison.OrdinalIgnoreCase))
            {
                return AuthorRole.System;
            }
            else if(roleString.Equals(nowSession.AssistantName,StringComparison.OrdinalIgnoreCase))
            {
                return AuthorRole.Assistant;
            }
            else
            {
                return AuthorRole.Unknown;
            }
        }

        async void UpdateText(string text = "Memory updated.")
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeOutAnimation);
            Storyboard.SetTarget(fadeOutAnimation, selectModel);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(TextBlock.OpacityProperty));

            storyboard.Begin();
            await Task.Delay(2000);
            selectModel.Text = text;

            storyboard = new Storyboard();
            storyboard.Children.Add(fadeInAnimation);
            Storyboard.SetTarget(fadeInAnimation, selectModel);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(TextBlock.OpacityProperty));

            storyboard.Begin();
            await Task.Delay(2000);
            selectModel.Text = $"Model: {nowSession.ModelName}";
        }

        private async void ChatSessions_SelectionChanged(iNKORE.UI.WPF.Modern.Controls.NavigationView sender, iNKORE.UI.WPF.Modern.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            DialogStackPanel.Children.Clear();
            foreach (var item in sessions)
            {
                if (ChatSessions.SelectedItem.ToString().Contains(item.Name))
                {
                    nowSession = item;
                    UpdateText($"Name: [{item.Name}] ID: [{item.ID}] ModelName: [{item.ModelName}]");
                    //MessageBox.Show($"Name: {item.Name}\nID: {item.ID}\nModelName: {item.ModelName}\nCreatDate: {item.CreatDate}");
                    await InitializeModelAsync();
                }
            }
            if (nowSession != null)
            {
                selectModel.Text = $"Model: {nowSession.ModelName}";
            }
        }
    }
}
