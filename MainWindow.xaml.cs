﻿// References
// https://gemini.google.com
// https://learn.microsoft.com/en-us/dotnet/api/system.eventhandler?view=netframework-4.8
// https://learn.microsoft.com/en-us/dotnet/api/system.windows.messageboxbutton?view=netframework-4.8


using ST10445832_PROG6221_PoE.Classes;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ST10445832_PROG6221_PoE
{
    public partial class MainWindow : Window
    {
        private string _userName;
        private Panel _currentPanel;
        private ChatBot _secWiz;

        public ObservableCollection<ChatMessage> ChatMessages { get; set; }

        public DispatcherTimer Timer { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ChatMessages = new ObservableCollection<ChatMessage>();
            this.DataContext = this;
            IntroPanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Collapsed;
            HelpPanel.Visibility = Visibility.Collapsed;
            WelcomePanel.Visibility = Visibility.Collapsed;
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
            IntroPanel.Visibility = Visibility.Visible;
            ChangeViewSwipe(BootPanel, IntroPanel);
        }

        private void UserNameContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!(UserNameInput.Text.Trim().Length > 0))
            {
                MessageBox.Show("Please enter a valid name.", "Invalid Name");
            }
            else
            {
                _userName = UserNameInput.Text;
                _secWiz = new ChatBot(_userName);
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
                ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = $"{_secWiz.Greeting()}" });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage();
            }
            else
            {
                ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = $"Hello {_userName}. What would you like to know?" });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage();
            }
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
            ChatInputBox.IsEnabled = false;
            SendMessageBtn.IsEnabled = false;
            // User question
            string question = ChatInputBox.Text;
            // clear question input field
            if (question.Length > 0)
            {
                ChatInputBox.Text = "";
                // show user question in UI
                ChatMessages.Add(new ChatMessage() { Author = $"{_userName}", Text = $"{question}" });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage(() => { ChatScrollViewer.ScrollToEnd(); });

                // Answer question and display in UI
                ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = $"\r{_secWiz.ProcessInput(question)}" });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage(() => { ChatScrollViewer.ScrollToEnd(); });
            }
            else
            {
                ChatMessages.Add(new ChatMessage() { Author = "SecWiz", Text = "You must type something for me to be able to answer." });
                await ChatMessages[ChatMessages.Count - 1].TypeMessage(() => { ChatScrollViewer.ScrollToEnd(); });
            }

            ChatInputBox.IsEnabled = true;
            SendMessageBtn.IsEnabled = true;
        }


        //================================================================================//
        // Facilitates the smooth transition between views
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
                    InitialiseTimer();
                }
            };

            swipeOut.Begin();
            swipeIn.Begin();
            next.Visibility = Visibility.Visible;
        }


        //=================== NOTIFICATIONS ========================//
        private void InitialiseTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += CheckNotifications;
            Timer.Start();
            Debug.WriteLine("timer initialised");
        }

        private void CheckNotifications(object sender, EventArgs e)
        {
            var notifications = _secWiz._botData.Tasks.Where(task => task.Reminder.ToString("yyyy/MM/dd HH:mm:ss").Equals(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))).ToList();

            if (notifications.Count > 0)
            {
                foreach (var task in notifications)
                {
                    var decision = MessageBox.Show($"Task: {task.Title}\n\nWould you like to delete this task?", $"Task Number: {task.TaskNumber}", MessageBoxButton.YesNo);
                    if (decision == MessageBoxResult.Yes)
                    {
                        _secWiz._botData.Tasks.Remove(task);
                        _secWiz._botData.UpdateTasks();
                    }
                    else
                    {
                        var indexToUpdate = _secWiz._botData.Tasks.IndexOf(_secWiz._botData.Tasks.Where(t => t.TaskNumber == task.TaskNumber).First());
                        task.Completed = true;
                        _secWiz._botData.Tasks[indexToUpdate] = task;
                        _secWiz._botData.UpdateTasks();
                    }
                }
            }
        }

    }
}
////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\