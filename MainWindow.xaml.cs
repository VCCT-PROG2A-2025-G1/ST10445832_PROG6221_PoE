// References
// https://gemini.google.com
// https://learn.microsoft.com/en-us/dotnet/api/system.eventhandler?view=netframework-4.8
// https://learn.microsoft.com/en-us/dotnet/api/system.windows.messageboxbutton?view=netframework-4.8



using ST10445832_PROG6221_POE_GUI.Classes;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Xml.Serialization;

namespace ST10445832_PROG6221_POE_GUI
{
    public partial class MainWindow : Window
    {
        private string _userName;
        private Panel _currentPanel;
        private ChatBot _secWiz;
        private BotData _botData;
        private Random _random = new Random();

        private string TasksDataPath = "Tasks.xml";
        public ObservableCollection<TaskReminder> Tasks { get; set; }
        public ObservableCollection<ChatMessage> ChatMessages { get; set; }
        public MainWindow()
        {
            Tasks = new ObservableCollection<TaskReminder>();
            ChatMessages = new ObservableCollection<ChatMessage>();
            this.DataContext = this;
            InitializeComponent();
            LoadTasks();
            IntroPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Collapsed;
            HelpPanel.Visibility = Visibility.Collapsed;
            WelcomePanel.Visibility = Visibility.Collapsed;
        }

        private void LoadTasks()
        {
            if (File.Exists(TasksDataPath))
            {
                try
                {
                    XmlSerializer xmlSerial = new XmlSerializer(typeof(ObservableCollection<TaskReminder>));
                    using (FileStream fs = new FileStream(TasksDataPath, FileMode.Open))
                    {
                        var tasks = (ObservableCollection<TaskReminder>) xmlSerial.Deserialize(fs);
                        foreach(TaskReminder task in tasks)
                        {
                            Tasks.Add(task);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void Title_Start(object sender, RoutedEventArgs e)
        {
            // Gemini
            Storyboard titleAnimation = ((Storyboard)FindResource("BootImageAnimation")).Clone();
            Storyboard.SetTarget(titleAnimation, this.BootImage);
            titleAnimation.Begin();
            // End Gemini
        }
        private void BootPanel_Ended(object sender, RoutedEventArgs e)
        {
            BootPanel.Visibility = Visibility.Collapsed;
            TitleLabel.Visibility = Visibility.Visible;
            IntroPanel.Visibility = Visibility.Visible;
        }

        private void UserNameContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!(UserNameInput.Text.Trim().Length > 0))
            {
                MessageBox.Show("Please enter a valid name.", "Invalid Input");
            }
            else
            {
                _userName = UserNameInput.Text;
                _secWiz = new ChatBot(_userName);
                _botData = new BotData(_userName);
                UserWelcomeLabel.Content = $"Hello {_userName}, welcome to SecWiz!";
                ChangeViewSwipe(IntroPanel, WelcomePanel);
            }
        }

        private async void Chat_Click(object sender, RoutedEventArgs e)
        {
            MenuPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Visible;
            _currentPanel = ChatPanel;
            
            if (ChatMessages.Count > 0)
            {
                ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = $"Welcome back {_userName}. {_botData.FirstMessageEndings[_random.Next(0, _botData.FirstMessageEndings.Count)]}" });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage();
            }
            else
            {
                ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = $"Hello {_userName}. What would you like to know?" });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage();
            }
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            MenuPanel.Visibility = Visibility.Collapsed;
            TaskPanel.Visibility = Visibility.Visible;
            _currentPanel = TaskPanel;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MenuPanel.Visibility = Visibility.Collapsed;
            HelpPanel.Visibility = Visibility.Visible;
            _currentPanel = HelpPanel;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var choice = MessageBox.Show($"Are you sure you want to quit, {_userName}?", "Confirm Exit", MessageBoxButton.YesNo);
            if (choice == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _currentPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;
        }

        //=================================================================================//
        // Click event handler for SendMessage Button in ChatPanel
        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            // User question
            string question = ChatInputBox.Text;
            // clear question input field
            ChatInputBox.Text = "";
            // show user question in UI
            ChatMessages.Add(new ChatMessage() { Author=$"{_userName}", Text=$"{question}" });
            await ChatMessages[ChatMessages.Count - 1].TypeMessage(() => { ChatScrollViewer.ScrollToEnd(); });

            // Answer question and display in UI
            ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = $"\r{_secWiz.AnswerQuestion(question)}" });
            await ChatMessages[ChatMessages.Count - 1].TypeMessage(() => { ChatScrollViewer.ScrollToEnd(); });
        }

        private void ChangeViewSwipe(Panel current, Panel next)
        {
            Storyboard swipeIn = ((Storyboard)FindResource("ScreenSwipeIn")).Clone();
            Storyboard.SetTarget(swipeIn, next);
            Storyboard swipeOut = ((Storyboard)FindResource("ScreenSwipeOut")).Clone();
            Storyboard.SetTarget(swipeOut, current);

            // hide the panel after swipe
            swipeOut.Completed += async (sender, _event) =>
            {
                current.Visibility = Visibility.Collapsed;
                if (next.Name == "WelcomePanel")
                {
                    await Task.Delay(1000);
                    ChangeViewSwipe(WelcomePanel, MenuPanel);
                }
            };

            swipeOut.Begin();
            swipeIn.Begin();
            next.Visibility = Visibility.Visible;
        }
    }
}
////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\