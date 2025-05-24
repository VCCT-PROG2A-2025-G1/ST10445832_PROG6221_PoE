// References
// https://www.asciiart.eu/image-to-ascii
// https://www.asciiart.eu/text-to-ascii-art
// https://chatgpt.com/
// https://gemini.google.com/
// https://learn.microsoft.com/en-us/dotnet/api/system.console.readkey?view=net-9.0
// https://learn.microsoft.com/en-us/dotnet/api/system.consolekey?view=net-9.0
// https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer?view=windowsdesktop-9.0


using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST10445832_PROG6221_PoE
{
    class MenuService
    {
        // Chat bot
        private Bot SecWiz;
        // audio player
        private SoundPlayer AudioPlayer = new SoundPlayer();
        // flag variable for audio player
        private bool IsPlaying = false;
        // user's name
        public string UserName;
        // flag variable for menu loop
        private bool Exit = false;
        // main menu navigation variables
        private int CursorPosition = 0;
        // ascii art to display above each menu
        private string[] MenuHeaderText =
        {
            " ____          __        ___     ",
            "/ ___|  ___  __\\ \\      / (_)____",
            "\\___ \\ / _ \\/ __\\ \\ /\\ / /| |_  /",
            " ___) |  __/ (__ \\ V  V / | |/ / ",
            "|____/ \\___|\\___| \\_/\\_/  |_/___|"
        };

        private string MenuHeader;
        // Title screen ASCII art
        private string logo =
            "###############################################################################" +
            "\n############################++++++++++++++++++++++++++#########################" +
            "\n#########################+++++++++++#######++++---------#######################" +
            "\n########################++++++++++####+++####------------######################" +
            "\n######################+++++++++++###+++----+##-----------######################" +
            "\n######################+++++++++++##++-------##-----------######################" +
            "\n######################+++++++++++##---------##-----------######################" +
            "\n######################+++++++++++++++++++++++++----------######################" +
            "\n######################++++++++++###############+---------######################" +
            "\n######################++++++++-+##+###+-+###+##+---------######################" +
            "\n######################+++++++--+######+-+######+---------######################" +
            "\n######################+++++++---###+###-###+###----------######################" +
            "\n######################++++++----+#############+----------######################" +
            "\n######################++++++------#####+#####------------######################" +
            "\n######################+++++---------#######--------------######################" +
            "\n#######################++-------------------------------#######################" +
            "\n#########################+----------------------------#########################" +
            "\n#################################---------++###################################" +
            "\n#################################-------+######################################" +
            "\n#################################-----+########################################" +
            "\n#################################---+##########################################" +
            "\n#################################-#############################################" +
            "\n###############################################################################";

        // Buttons
        private string MenuButtons =
            "    ==================        ==================        ==================\n" +
            "    =                =        =                =        =                =\n" +
            "    =      CHAT      =        =      HELP      =        =      EXIT      =\n" +
            "    =                =        =                =        =                =\n" +
            "    ==================        ==================        ==================";
        // Highlighted button design
        private string[] ChatBtnArr =
        {
            "                  ",
            " ================ ",
            " =     CHAT     = ",
            " ================ ",
            "                  "
        };

        private string[] HelpBtnArr =
        {
            "                  ",
            " ================ ",
            " =     HELP     = ",
            " ================ ",
            "                  "
        };

        private string[] ExitBtnArr =
        {
            "                  ",
            " ================ ",
            " =     EXIT     = ",
            " ================ ",
            "                  "
        };


        //=========================================================//
        // Default constructor
        public MenuService() { }


        //=========================================================//
        // Calls:
        // ConfigureConsole
        // StartUpGreeting
        // StartUpAnimation
        // BuilMainMenuString
        // Clears the title screen and resets the cursor
        public void RunStartUpSequence()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            // set console dimensions
            ConfigureConsole();
            // build menu header
            CreateMenuHeader("Welcome");
            // start audio greeting
            StartUpGreeting();
            // display title screen animation
            StartUpAnimation();
            // wait for audio to stop before continuing
            while (IsPlaying) ;
            // Clear away the title screen
            Console.Clear();
            // Reset the cursor position
            Console.SetCursorPosition(0, 0);
            // Request username and clear the console
            RequestUserName();
            // Create Chat bot instance with user name
            SecWiz = new Bot(UserName);
            Console.Clear();
            // display main menu
            MainMenu();
        }


        //====================== Menu Methods ======================//

        //=========================================================//
        // Displays the main menu and prompts navigation input
        private void MainMenu()
        {
            // show header
            CreateMenuHeader("Main Menu");
            Console.Write(MenuHeader);
            // show menu options
            Console.Write(MenuButtons);
            // highlight first button
            DrawMenuButtons();

            while (!Exit)
            {
                var inputKey = Console.ReadKey(true);
                if (inputKey.Key == ConsoleKey.LeftArrow)
                {
                    // move cursor position
                    MoveCursorLeft();
                    // highlight menu option
                    DrawMenuButtons();
                }
                else if (inputKey.Key == ConsoleKey.RightArrow)
                {
                    // move cursor position
                    MoveCursorRight();
                    // highlight menu option
                    DrawMenuButtons();
                }
                else if (inputKey.Key == ConsoleKey.Enter)
                {
                    SelectMenuOption("main");
                }
            }
        }


        //=========================================================//
        // Prompt the user for a question
        // Answer the question
        // Until the user indicates they have no more questions
        private void QuestionLoop(string question)
        {
            bool toMain = false;

            while (!toMain)
            {
                // get an answer
                List<string> answer = SecWiz.AnswerQuestion(question);
                if (answer.Contains("Thanks for chatting!"))
                {
                    Console.Write($"\rSecWiz: ");
                    TypeMessage("Thanks for chatting!");
                    Console.Write("\n");
                    Thread.Sleep(1000);
                    toMain = true;
                    break;
                }
                // ...
                ShowThinking();
                // display answer
                foreach (string line in answer)
                {
                    Console.Write($"\rSecWiz: ");
                    TypeMessage(line);
                    Console.Write("\n");
                }
                Console.Write($"{UserName}: ");
                // update question variable with new question
                question = Console.ReadLine();
            }
            TypeMessage($"SecWiz: Goodbye, {UserName}!");
            Thread.Sleep(1000);
            // prepare the console for the main menu
            Console.Clear();
            Console.CursorVisible = false;
            // go to the main menu
            MainMenu();
        }


        //=========================================================//
        // Displays information to help the user use SecWiz
        private void HelpMenu()
        {
            Console.Clear();
            CreateMenuHeader("Help");
            Console.Write(MenuHeader);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Main Menu");
            sb.AppendLine("\tChat: chat with SecWiz to learn about cybersecurity");
            sb.AppendLine("\tHelp: you are here! I hope you are being helped");
            sb.AppendLine("\tExit: closes the application");
            sb.AppendLine();
            sb.AppendLine("Chat");
            sb.AppendLine("\tType any question you have, when prompted, and SecWiz will\n\ttry to answer you.");
            sb.AppendLine("\tTo return to the main menu, enter exit, quit, goodbye, or \n\tno more questions.");
            sb.AppendLine();
            sb.AppendLine("Press any key to return to the main menu...");
            Console.WriteLine(sb.ToString());
            Console.ReadKey(true);
            // prepare console for main menu
            Console.Clear();
            SelectMenuOption("help");
        }


        //====================== Menu Control Methods ======================//

        //=========================================================//
        // Moves the menu option highlight cursor left
        private void MoveCursorLeft()
        {
            if (CursorPosition < 3 && CursorPosition > 0)
            {
                CursorPosition--;
            }
        }


        //=========================================================//
        // Moves the menu option highlight cursor right
        private void MoveCursorRight()
        {
            if (CursorPosition >= 0 && CursorPosition < 2)
            {
                CursorPosition++;
            }
        }


        //=========================================================//
        // Highlights button 1 2 or 3 based on CursorPosition field
        private void DrawMenuButtons()
        {
            // ChatBtn Coordinates X: 4 - 21, Y: 10 - 14
            // HelpBtn Coordinates X: 30 - 47, Y: 10 - 14
            // ExitBtn Coordinates X: 56 - 73, Y: 10 - 14

            // Reset highlight
            Console.SetCursorPosition(0, 10);
            Console.Write(MenuButtons);

            // highlight button for user to select
            // set highlight colours
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            if (CursorPosition == 0)
            {
                int row = 0;
                foreach (string line in ChatBtnArr)
                {
                    Console.SetCursorPosition(4, 10 + row);
                    Console.Write(line);
                    row++;
                }
            }
            else if (CursorPosition == 1)
            {
                int row = 0;
                foreach (string line in HelpBtnArr)
                {
                    Console.SetCursorPosition(30, 10 + row);
                    Console.Write(line);
                    row++;
                }
            }
            else
            {
                int row = 0;
                foreach (string line in ExitBtnArr)
                {
                    Console.SetCursorPosition(56, 10 + row);
                    Console.Write(line);
                    row++;
                }
            }
            // Reset the colours
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
        }


        //=========================================================//
        // Handles menu navigation
        private void SelectMenuOption(string menuName)
        {
            if (menuName.Equals("main"))
            {
                switch (CursorPosition)
                {
                    case 0:
                        QuestionLoop(TakeQuestion());
                        break;
                    case 1:
                        HelpMenu();
                        break;
                    case 2:
                        Exit = true;
                        break;
                }
            }
            else if (menuName.Equals("help"))
            {
                MainMenu();
            }
        }


        //==================== Startup Methods ====================//

        //=========================================================//
        // Plays an audio greeting
        private void StartUpGreeting()
        {

            AudioPlayer.SoundLocation = "SecWizTelephone.wav";

            try
            {
                // ChatGPT
                Task.Run(() =>
                {
                    IsPlaying = true;
                    AudioPlayer.Load();
                    AudioPlayer.PlaySync();
                    IsPlaying = false;
                });
                // End ChatGPT
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //=========================================================//
        // Hides the cursor, sets the console size
        private void ConfigureConsole()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(79, 23);
            Console.SetBufferSize(79, 23);
        }


        //=========================================================//
        // Build menu header with centered ascii art and menu title
        // for user orientation
        private void CreateMenuHeader(string menuName)
        {
            StringBuilder sb = new StringBuilder();
            int width = Console.WindowWidth;
            // ChatGPT
            int paddingWidth = (width - MenuHeaderText[0].Length) / 2; // all lines are equal length
            var padding = new string(' ', paddingWidth);
            // End ChatGPT

            // add top border
            sb.AppendLine(new string('#', width));
            sb.AppendLine();
            foreach (string line in MenuHeaderText)
            {
                sb.AppendLine($"{padding}{line}{padding}");
            }
            sb.AppendLine();
            // add bottom border
            if (menuName == "")
            {
                sb.AppendLine(new string('#', width));
            }
            else
            {
                paddingWidth = ((width - menuName.Length) / 2) - 1;
                padding = new string('#', paddingWidth);
                string border = $"{padding} {menuName} {padding}";
                while (border.Length < 79)
                {
                    border += "#";
                }
                sb.AppendLine($"{border}");
            }
            sb.AppendLine();
            MenuHeader = sb.ToString();
        }


        //=========================================================//
        // Displays the animated title screen with cascading effect
        private void StartUpAnimation()
        {
            foreach (string line in logo.Split('\n'))
            {
                Console.Write(line);
                Thread.Sleep(30);
            }
        }


        //=========================================================//
        // Prompts the user for their name and saves it in memory
        private void RequestUserName()
        {
            Console.Write(MenuHeader);
            TypeMessage("Hello, I'm SecWiz! Before we get started, please enter your name.");
            Console.CursorVisible = true;
            Console.Write("\n>>>");
            UserName = Console.ReadLine();
            while (UserName.Trim().Equals(""))
            {
                TypeMessage("Please enter a valid name.");
                Console.Write("\n>>>");
                UserName = Console.ReadLine();
            }

            Console.CursorVisible = false;
            Console.Clear();
            Console.Write(MenuHeader);
            ShowThinking();
            TypeMessage($"\rThank you, {UserName}! Let's learn!");
            Console.Write("\n\n");
            ShowThinking();
            TypeMessage("\rUse the LEFT and RIGHT arrow keys to navigate the main menu.");
            Thread.Sleep(200);
            TypeMessage("\nPress any key to continue to the main menu...");
            Console.ReadKey(true);
            Console.Clear();
            Console.Write(MenuHeader);
        }


        //==================== Helper Methods =====================//

        //=========================================================//
        // Prompt user to ask a question
        // Acts as priming input for question loop
        private string TakeQuestion()
        {
            var menu = "What would you like to know?\n" +
                       $"{UserName}: ";
            Console.Clear();
            CreateMenuHeader("Chat");
            Console.Write(MenuHeader);
            Console.CursorVisible = true;
            Console.Write(menu);
            Console.CursorVisible = true;
            var question = Console.ReadLine();

            return question;
        }


        //=========================================================//
        // Prints three dots to feign thinking
        private void ShowThinking()
        {
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(300);
                Console.Write(".");
            }
            Thread.Sleep(400);
        }


        //=========================================================//
        // Simulates typing
        private void TypeMessage(string message)
        {
            string[] messageArr = message.Split(' ');
            foreach (string word in messageArr)
            {
                Thread.Sleep(word.Length * 50);
                Console.Write($"{word} ");
            }
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\