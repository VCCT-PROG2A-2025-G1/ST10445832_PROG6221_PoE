// References
// https://github.com/JakeBayer/FuzzySharp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzySharp;

namespace ST10445832_PROG6221_Part1
{
    internal class Bot
    {
        Dictionary<string, string> QnA = new Dictionary<string, string>();

        //=========================================================//
        // Default Constructor
        public Bot()
        {
            InitialiseQnA();
        }


        //=========================================================//
        // Populate the QnA dictionary with questions and answers
        private void InitialiseQnA()
        {
            // DEFAULT
            QnA.Add("", "I'm sorry, I don't understand your question. Try to rephrase it or ask me something else.");

            // ASSISTANCE
            QnA.Add("Help", "What do you want help with? You can ask me about anything cybersecurity related!");
            QnA.Add("What can I ask you?", "You can ask me any cybersecurity related question and I will do my best to answer you.");
            QnA.Add("What can I ask you about?", "You can ask me any cybersecurity related question and I will do my best to answer you.");

            // OFF-TOPIC
            QnA.Add("What is your name?", "My name is SecWiz!");
            QnA.Add("How are you?", "I am well, thank you. I'm always ready to answer your questions! Ask away!");
            QnA.Add("What's your purpose?", "I'm here to educate users on matters relating to cyber security and online safety.");
            QnA.Add("Thank you", "My pleasure. Is there anything else you would like to know?");

            // CYBERSECURITY
            QnA.Add("What is a VPN?", "VPN stands for Virtual Private Network. It's a service that creates a secure, encrypted connection over the internet between your device and a remote server, helping to protect your online privacy and data.");
            QnA.Add("What is a virtual private network?", "A virtual private network, or VPN, is a service that creates a secure, encrypted connection over the internet between your device and a remote server, helping to protect your online privacy and data.");

            // EXIT
            QnA.Add("No more questions", "Thanks for chatting!");
            QnA.Add("No", "Thanks for chatting!");
            QnA.Add("No thanks", "Thanks for chatting!");
            QnA.Add("Goodbye", "Thanks for chatting!");
            QnA.Add("Bye", "Thanks for chatting!");
            QnA.Add("Back", "Thanks for chatting!");
        }


        //=========================================================//
        // Use the FuzzySharp package to return an answer based
        // on a similarity score, comparing the user question to
        // the questions in the QnA dictionary
        public string AnswerQuestion(string userQuestion)
        {
            // best match score
            int fuzzMax = 0;
            // best match question
            string bestMatch = "";
            foreach (string question in QnA.Keys)
            {
                // compare the question to questions which have answers, ignoring the order of the words in the question
                var score = Fuzz.TokenSortRatio(userQuestion, question);
                // only provide a non-default answer if the similarity score is higher than 60
                if (score > 60 && score > fuzzMax)
                {
                    fuzzMax = score;
                    bestMatch = question;
                }
            }
            // return the most relevant answer
            return QnA[bestMatch];
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\