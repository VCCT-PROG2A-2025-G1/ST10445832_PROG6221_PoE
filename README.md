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
__IMPORTANT__: reducing the console window width will mess up the UI, though the application functionality will remain.

### Main Menu
Highlight the menu option you would like to select using __ArrowLeft__ and __ArrowRight__.
Use __Enter__ to select the highlighted option.

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
### Help
- The help option in the main menu provides you with some information about SecWiz and how to use it.

### Exit
- Exit does what it says - it closes the application.


## SecWiz Walkthrough/Demonstration
Part 1: [https://youtu.be/hN8oyaWWZ0A] <br>
Part 2: [https://youtu.be/91BsgoXmWG8]
