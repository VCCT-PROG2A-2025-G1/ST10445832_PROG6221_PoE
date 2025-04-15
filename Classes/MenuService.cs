// References
// https://chatgpt.com/
// https://learn.microsoft.com/en-us/dotnet/api/system.console.readkey?view=net-9.0
// https://learn.microsoft.com/en-us/dotnet/api/system.consolekey?view=net-9.0
// https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer?view=windowsdesktop-9.0


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST10445832_PROG6221_Part1
{
    class MenuService
    {
        private Bot SecWiz = new Bot();
        private SoundPlayer AudioPlayer = new SoundPlayer();
        private bool IsPlaying = false;
        public string UserName;
        private bool Exit = false;
        private int CursorPosition = 0;
        private string[] MainMenuOptions = { "\rAsk Question", "\rHelp", "\rExit" };
        private string MenuHeader = "################################################################################\n" +
                                    "============   _____ ______     _____  __      __    ______   _____ ============\n" +
                                    "############ / ____/ #-----   /  ___/ |  |    |  | /_    _/ /___  / ############\n" +
                                    "============ \\__  \\  #_____  |  /     |  |_/\\_|  |   |  |     /  /  ============\n" +
                                    "############ ___/ /  #_____  |  \\___  \\    /\\    /  _|  |_   /  /__ ############\n" +
                                    "============/____/   #-----   \\____/   \\__/  \\__/  /_____/  /_____/ ============\n" +
                                    "############                                                        ############\n" +
                                    "================================================================================\n" +
                                    "################################################################################\n\n";

        //=========================================================//
        // Default constructor
        public MenuService() { }


        //=========================================================//
        // Displays the main menu and prompts navigation input
        private void MainMenu()
        {
            var menu = "Ask Question\n" +
                       "Help\n" +
                       "Exit";

            Console.Write(MenuHeader);
            Console.Write(menu);
            DrawCursor("main");

            while (!Exit)
            {
                var inputKey = Console.ReadKey(true);
                if (inputKey.Key == ConsoleKey.UpArrow)
                {
                    // move cursor position
                    MoveCursorUp();
                    // highlight menu option
                    DrawCursor("main");
                }
                else if (inputKey.Key == ConsoleKey.DownArrow)
                {
                    // move cursor position
                    MoveCursorDown();
                    // highlight menu option
                    DrawCursor("main");
                }
                else if (inputKey.Key == ConsoleKey.Enter)
                {
                    SelectMenuOption("main");
                }
            }
        }

        //=========================================================//
        // Displays information to help the user use SecWiz
        private void HelpMenu()
        {
            Console.Write(MenuHeader);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Main Menu");
            sb.AppendLine("\tAsk Question: chat with SecWiz to learn about cybersecurity");
            sb.AppendLine("\tHelp: you are here! I hope you are being helped");
            sb.AppendLine("\tExit: closes the application");
            sb.AppendLine();
            sb.AppendLine("Ask Question");
            sb.AppendLine("\tType any question you have, when prompted, and SecWiz will try to answer you.");
            sb.AppendLine("\tTo return to the main menu, enter exit, quit, goodbye, or no more questions.");
            sb.AppendLine();
            sb.AppendLine("Press any key to return to the main menu...");
            Console.WriteLine(sb.ToString());
            Console.ReadKey(true);
            // prepare console for main menu
            Console.Clear();
            SelectMenuOption("help");
        }


        //=========================================================//
        // Moves the menu option highlight cursor up
        private void MoveCursorUp()
        {
            if (CursorPosition < 3 && CursorPosition > 0)
            {
                // Reset currently highlighted option
                Console.Write(MainMenuOptions[CursorPosition]);
                CursorPosition--;
            }
        }


        //=========================================================//
        // Moves the menu option highlight cursor down
        private void MoveCursorDown()
        {
            if (CursorPosition >= 0 && CursorPosition < 2)
            {
                Console.Write(MainMenuOptions[CursorPosition]);
                CursorPosition++;
            }
        }


        //=========================================================//
        // Draws the menu option highlight cursor
        private void DrawCursor(string menuName)
        {
            switch (menuName)
            {
                case "main":
                    Console.SetCursorPosition(0, (10 + CursorPosition));
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(MainMenuOptions[CursorPosition]);
                    // reset the console colour
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }
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
                        Environment.Exit(0);
                        break;
                }
            }
            else if (menuName.Equals("help"))
            {
                MainMenu();
            }
        }


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
            ConfigureConsole();
            StartUpGreeting();
            StartUpAnimation();
            // wait for audio to stop before continuing
            while (IsPlaying) ;
            // Clear away the title screen
            Console.Clear();
            // Reset the cursor
            Console.SetCursorPosition(0, 0);
            // Request username and clear the console
            RequestUserName();
            Console.Clear();
            MainMenu();
        }


        //=========================================================//
        // Prompts the user for their name and saves it in memory
        private void RequestUserName()
        {
            Console.Write(MenuHeader);
            Console.WriteLine("Hello, I'm SecWiz! Before we get started, please enter your name.");
            Console.CursorVisible = true;
            Console.Write(">>>");
            UserName = Console.ReadLine();
            while (UserName.Trim().Equals(""))
            {
                Console.WriteLine("Please enter a valid name.");
                Console.Write(">>>");
                UserName = Console.ReadLine();
            }

            Console.CursorVisible = false;
            Console.Clear();
            Console.Write(MenuHeader);
            Thread.Sleep(500);
            Console.WriteLine($"Thank you, {UserName}! Let's learn!");
            Thread.Sleep(1000);
            Console.Clear();
            Console.Write(MenuHeader);
            ShowThinking();
        }


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
            Console.SetWindowSize(81, 20);
            Console.SetBufferSize(81, 20);
        }


        //=========================================================//
        // Displays the animated title screen
        private void StartUpAnimation()
        {
            /* 
                 _____ ______     _____  __      __    ______   _____
               / ____/ #-----   /  ___/ |  |    |  | /_    _/ /___  /
               \__  \  #_____  |  /     |  |_/\_|  |   |  |     /  /
               ___/ /  #_____  |  \___  \    /\    /  _|  |_   /  /__
              /____/   #-----   \____/   \__/  \__/  /_____/  /_____/
             
             */
            var titleRowOne = "   _____ ______     _____  __      __    ______   _____";
            var titleRowTwo = " / ____/ #-----   /  ___/ |  |    |  | /_    _/ /___  /";
            var titleRowThree = " \\__  \\  #_____  |  /     |  |_/\\_|  |   |  |     /  /\n";
            var titleRowFour = " ___/ /  #_____  |  \\___  \\    /\\    /  _|  |_   /  /__";
            var titleRowFive = "/____/   #-----   \\____/   \\__/  \\__/  /_____/  /_____/";
            string[] titleRows = { titleRowOne, titleRowTwo, titleRowThree, titleRowFour, titleRowFive };
            var titleBackground = "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "============                                                        ============\n" +
                                  "############                                                        ############\n" +
                                  "============                                                        ============\n" +
                                  "############                                                        ############\n" +
                                  "============                                                        ============\n" +
                                  "############                                                        ############\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################";

            var titleBackgroundGlitch1 = "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "===========   _____ ______     _____  __      __    ______   _____ =============\n" +
                                  "############# / ____/ #-----   /  ___/ |  |    |  | /_    _/ /___  / ###########\n" +
                                  "============= \\__  \\  #_____  |  /     |  |_/\\_|  |   |  |     /  /  ===========\n" +
                                  "########### ___/ /  #_____  |  \\___  \\    /\\    /  _|  |_   /  /__ #############\n" +
                                  "==========/____/   #-----   \\____/   \\__/  \\__/  /_____/  /_____/ =============\n" +
                                  "############                                                        ############\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################";

            var titleBackgroundGlitch2 = "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "============   _____ ______     _____  __      __    ______   _____ ============\n" +
                                  "########## / ____/ #-----   /  ___/ |  |    |  | /_    _/ /___  / ##############\n" +
                                  "============= \\__  \\  #_____  |  /     |  |_/\\_|  |   |  |     /  / ============\n" +
                                  "############ ___/ /  #_____  |  \\___  \\    /\\    /  _|  |_   /  /__ ############\n" +
                                  "=============/____/   #-----   \\____/   \\__/  \\__/  /_____/  /_____/============\n" +
                                  "############                                                        ############\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################";

            var titleBackgroundComplete = "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                    "============   _____ ______     _____  __      __    ______   _____ ============\n" +
                                    "############ / ____/ #-----   /  ___/ |  |    |  | /_    _/ /___  / ############\n" +
                                    "============ \\__  \\  #_____  |  /     |  |_/\\_|  |   |  |     /  /  ============\n" +
                                    "############ ___/ /  #_____  |  \\___  \\    /\\    /  _|  |_   /  /__ ############\n" +
                                    "============/____/   #-----   \\____/   \\__/  \\__/  /_____/  /_____/ ============\n" +
                                    "############                                                        ############\n" +
                                    "================================================================================\n" +
                                    "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################\n" +
                                  "================================================================================\n" +
                                  "################################################################################";

            // Display background
            Console.Write(titleBackground);
            // Delay logo write
            Thread.Sleep(100);
            // Set cursor position to prefare for first line of logo
            Console.SetCursorPosition(12, 6);
            // Write first line of logo
            Console.Write(titleRowOne);
            // Repeat
            for (int i = 1; i < titleRows.Length; i++)
            {
                Thread.Sleep(200);
                Console.SetCursorPosition(12, (i + 6));
                Console.Write(titleRows[i]);
            }

            Thread.Sleep(200);
            Console.Clear();
            // after logo has been displayed, glitch!
            // first glitch stage
            Console.Write(titleBackgroundGlitch1);
            Thread.Sleep(200);
            Console.Clear();
            // second glitch stage
            Console.Write(titleBackgroundGlitch2);
            Thread.Sleep(200);
            Console.Clear();
            // return to normal
            Console.Write(titleBackgroundComplete);
        }


        //=========================================================//
        // Prompt user to ask a question
        private string TakeQuestion()
        {
            var menu = "What would you like to know?\n" +
                       $"{UserName}: ";
            Console.Clear();
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
        // Prompt the user for a question
        // Answer the question
        // Until the user indicates they have no more questions
        private void QuestionLoop(string question)
        {
            bool toMain = false;

            while (!toMain)
            {
                // get an answer
                var answer = SecWiz.AnswerQuestion(question);
                // ...
                ShowThinking();
                // display answer
                Console.WriteLine($"\rSecWiz: {answer}");
                // allow user to ask another question if the 'exit' answer isn't returned
                if (answer.Equals("Thanks for chatting!"))
                {
                    toMain = true;
                }
                else
                {
                    Console.Write($"{UserName}: ");
                    question = Console.ReadLine();
                }
            }
            // prepare the console for the main menu
            Console.Clear();
            Console.CursorVisible = false;
            MainMenu();
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\