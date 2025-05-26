// References
// https://chatgpt.com
// https://github.com/JakeBayer/FuzzySharp

using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using ST10445832_PROG6221_PoE.Classes;

namespace ST10445832_PROG6221_PoE
{
    internal class Bot
    {
        private Data BotData;

        private int CurrentSentiment;
        private string UserInterest;
        private string CurrentTopic;
        private List<string> InputHistory = new List<string>();
        private List<string> AnswerHistory = new List<string>();

        private Random Rand = new Random();

        //=========================================================//
        // Default Constructor
        public Bot(string userName)
        {
            // Initialise data
            BotData = new Data(userName);
        }


        //=========================================================//
        // Return an answer based on a keyword, or a general match
        // if no known keyword is identified.
        public List<string> AnswerQuestion(string userQuestion)
        {
            // prepare question
            userQuestion = userQuestion.Trim().ToLower();
            // add question to history
            InputHistory.Add(userQuestion);

            // check for keyword
            var keywordMatch = Process.ExtractOne(userQuestion, BotData.Keywords);
            //  limit responses by keyword if found
            if (keywordMatch != null && keywordMatch.Score >= 60)
            {
                CurrentTopic = keywordMatch.Value;

                if (userQuestion.Contains("tip"))
                {
                    return GetRandomTip(userQuestion, CurrentTopic);
                }
                else
                {
                    var keywordAnswers = Process.ExtractAll(CurrentTopic, BotData.QnA.Keys)
                        .Where(question => question.Score > 75)
                        .Select(question => BotData.QnA[question.Value])
                        .ToList();
                    var choicesDict = BotData.QnA.Where(kvp => keywordAnswers.Contains(kvp.Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value); // Copilot
                    choicesDict.Add("", BotData.QnA[""]);

                    return GetBestMatch(userQuestion, choicesDict);
                }
            }

            return GetBestMatch(userQuestion, BotData.QnA);
        }


        //=========================================================//
        // Use the FuzzySharp package to return an answer based
        // on a similarity score, comparing the user question to
        // the questions in the QnA dictionary
        private List<string> GetBestMatch(string userQuestion, Dictionary<string, string> qDict)
        {
            List<string> outputList = new List<string>();

            // record the user's interest if stated
            if (userQuestion.Contains("interested"))
            {
                UserInterest = CurrentTopic;
                outputList.Add(BotData.GetInterestOpener(UserInterest));
            }

            var prevSentiment = CurrentSentiment;
            SetSentiment(userQuestion);

            // does the user appear confused by the previous answer?
            if ((prevSentiment != CurrentSentiment) && (CurrentSentiment == (int)Data.Sentiment.CONFUSED))
            {
                outputList.Add("I understand it can be confusing. Let me try to explain further.");
            }

            // best match score
            int fuzzMax = 0;
            // best match question
            string bestMatch = "";

            foreach (string question in qDict.Keys)
            {
                // compare the question to questions which have answers, ignoring the order of the words in the question
                var score = Fuzz.TokenSortRatio(userQuestion, question.ToLower());
                // only provide a non-default answer if the similarity score is higher than 60 or higher than 90 if the prompt is 3 words or fewer
                if (userQuestion.Split(' ').Count() > 4)
                {
                    if (score > 60 && score > fuzzMax)
                    {
                        fuzzMax = score;
                        bestMatch = question;
                    }
                }
                else
                {
                    if (score > 90 && score > fuzzMax)
                    {
                        fuzzMax = score;
                        bestMatch = question;
                    }
                }
            }

            if (!bestMatch.Equals(""))
            {
                // Add an opener
                if (UserInterest != null && userQuestion.Contains(UserInterest) && InputHistory.Count() > 2)
                {
                    outputList.Add(BotData.RecallInterest(UserInterest)[Rand.Next(0, 8)]);
                }
                else
                {
                    outputList.Insert(0, BotData.Openers[CurrentSentiment][Rand.Next(0, BotData.Openers[CurrentSentiment].Count())]);
                }
                // Add answer
                outputList.Add(qDict[bestMatch]);
                // Add follow up
                outputList.Add(GetFollowUp(""));
            }
            else
            {
                // Default response
                outputList.Add(qDict[bestMatch]);
            }
            AnswerHistory.Add(qDict[bestMatch]);
            return outputList;
        }


        //=========================================================//
        // Returns a random tip given a topic
        private List<string> GetRandomTip(string userQuestion, string keyword)
        {
            List<string> outputList = new List<string>();
            userQuestion = userQuestion.ToLower();
            SetSentiment(userQuestion);
            // Add an opener
            outputList.Insert(0, GetSentimentOpener());
            // Add tip if not the same as previous output
            int rand;
            string tip = "";
            do
            {
                rand = Rand.Next(0, BotData.Tips[$"{keyword} tip"].Count());
                tip = BotData.Tips[$"{keyword} tip"][rand];
                if (tip != AnswerHistory.Last())
                {
                    outputList.Add(tip);
                }
            } while (tip.Equals(AnswerHistory.Last()));
            AnswerHistory.Add(tip);
            // Add follow up
            outputList.Add(GetFollowUp(keyword));

            return outputList;
        }


        //=========================================================//
        // Returns a sentiment based opener
        private string GetSentimentOpener()
        {
            return BotData.Openers[CurrentSentiment][Rand.Next(0, BotData.Openers[CurrentSentiment].Count())];
        }


        //=========================================================//
        // Returns a random follow-up response
        private string GetFollowUp(string topic)
        {
            if (topic.Length == 0)
            {
                return BotData.FollowUps[Rand.Next(0, BotData.FollowUps.Count())];
            }
            else
            {
                return BotData.GetFollowUps(topic)[Rand.Next(0, BotData.FollowUps.Count())];
            }
        }


        //=========================================================//
        // Sets the current sentiment based on words contained in
        // the latest user input
        private void SetSentiment(string question)
        {
            // Default mood
            CurrentSentiment = (int)Data.Sentiment.NEUTRAL;
            foreach (string word in question.Split(' '))
            {
                if (word.Length > 3)
                {
                    if (Process.ExtractOne(word, BotData.SentimentWords[(int)Data.Sentiment.WORRIED]).Score >= 60)
                    {
                        CurrentSentiment = (int)Data.Sentiment.WORRIED;
                    }
                    else if (Process.ExtractOne(word, BotData.SentimentWords[(int)Data.Sentiment.CURIOUS]).Score >= 60)
                    {
                        CurrentSentiment = (int)Data.Sentiment.CURIOUS;
                    }
                    else if (Process.ExtractOne(word, BotData.SentimentWords[(int)Data.Sentiment.HAPPY]).Score >= 60)
                    {
                        CurrentSentiment = (int)Data.Sentiment.HAPPY;
                    }
                    else if (Process.ExtractOne(word, BotData.SentimentWords[(int)Data.Sentiment.FRUSTRATED]).Score >= 60)
                    {
                        CurrentSentiment = (int)Data.Sentiment.FRUSTRATED;
                    }
                    else if (Process.ExtractOne(word, BotData.SentimentWords[(int)Data.Sentiment.CONFUSED]).Score >= 60)
                    {
                        CurrentSentiment = (int)Data.Sentiment.CONFUSED;
                    }
                }
            }
        }

    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\