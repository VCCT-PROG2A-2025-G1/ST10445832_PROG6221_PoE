# ST10445832 PROG6221 PoE - SecWiz Chat Bot

## Running SecWiz
### Requirements
- Microsoft Visual Studio
- Nuget Package: FuzzySharp 2.0.2 by Jacob Bayer
  - Open ST10445832_PROG6221_PoE.sln in Visual Studio
  - Tools -> NuGet Package Manager -> Manage NuGet Packages for solution
  - Browse and install FuzzSharp if not under 'installed' tab
- Nuget Package: CommunityToolkit.Mvvm by Microsoft
- Nuget Package: MaterialDesignColors by James Willock
- Nuget Package: MaterialDesignThemes by James Willock

## SecWiz Usage
- For many commands, there is some flexibility in how you can make them, the examples below aren't necessarily the only way of doing things.
- At any point during a quiz round or task action, you can type _cancel_ to abort the process.

### Navigation
You can you your mouse or TAB and Enter keys to navigate the UI.

### Chat
- After selecting _CHAT_ in the main menu you will be prompted to ask a question.
- Any question you ask will be compared to answerable questions using fuzzy string matching.
> Example <br>
> User: Give me a password tip <br>
> SecWiz: Make strong passwords, using a combination of upper and lower case letters, numbers, and symbols.
- You can enter prompts such as _No more Questions_, _Goodbye_, _Back_, and _Exit_, to return to the __main menu__.
> Example <br>
> User: Goodbye! <br>
> SecWiz: Thanks for Chatting! <br>
> SecWiz: Goodbye >username<
- To begin the Quiz you can type _I want to play the quiz_. SecWiz will tell you the game instructions and then ask you when you want to start. Replying with _ready_ will start the quiz.
- To create a new task reminder type _Remind me to <task> <time period> from now_
> User: reminde me to update my password 20 minutes from now. <br>
> SecWiz: Task created! <br>
> Task: 'update my password', Reminder: 2025/06/25 12:30:00
- View a history of recent significant events with _show activity log_.

### Help
- The help option in the main menu provides you with some information about SecWiz and how to use it.

### Exit
- Exit does what it says - it closes the application.


## SecWiz Walkthrough/Demonstration
Part 1: [https://youtu.be/hN8oyaWWZ0A] <br>
Part 2: [https://youtu.be/91BsgoXmWG8]
