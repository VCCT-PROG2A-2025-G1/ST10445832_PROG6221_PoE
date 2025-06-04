// References
// https://gemini.google.com

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;

namespace ST10445832_PROG6221_PoE.Classes
{
    public class ChatMessage : ObservableObject
    {
        public string Author { get; set; }
        public string Text { get; set; }
        // Gemini
        private string _typingText;
        public string TypingText
        {
            get=> _typingText;
            set => SetProperty(ref _typingText, value);
        }
        // End Gemini


        //=========================================================//
        // Simulates typing
        // Gemini used
        public async Task TypeMessage(Action onTypingUpdated = null)
        {
            TypingText = "";
            if (Author == "SecWiz")
            {
                string[] messageArr = Text.Split(' ');
                foreach (string word in messageArr)
                {
                    await Task.Delay(word.Length * 50);
                    // with space if not first word
                    TypingText += (TypingText.Length > 0 ? " ": "") + word;
                    onTypingUpdated?.Invoke();
                }
            }
            else
            {
                TypingText += Text;
                onTypingUpdated?.Invoke();
            }
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\