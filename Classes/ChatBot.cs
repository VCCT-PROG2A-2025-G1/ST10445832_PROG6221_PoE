// References
// https://chatgpt.com
// https://github.com/JakeBayer/FuzzySharp

using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST10445832_PROG6221_PoE.Classes
{
    public class ChatBot
    {
        private BotData _botData;

        private int _currentSentiment;
        private string _userInterest;
        private string _currentTopic;
        private List<string> _inputHistory = new List<string>();
        private List<string> _answerHistory = new List<string>();

        private Random _rand = new Random();

        //=========================================================//
        // Default Constructor
        public ChatBot(string userName)
        {
            // Initialise data
            _botData = new BotData(userName);
        }


        //=========================================================//
        // Return an answer based on a keyword, or a general match
        // if no known keyword is identified.
        public string AnswerQuestion(string userQuestion)
        {
            userQuestion = userQuestion.Trim().ToLower();
            _inputHistory.Add(userQuestion);
            string answerString = "";

            if (userQuestion.Contains("add task"))
            {
                AddTask(userQuestion);
            }

            if (userQuestion.Contains("list tasks"))
            {
                foreach (var task in _botData.Tasks)
                {
                    answerString += task.ToString();
                }
            }

            // check for keyword
            var keywordMatch = Process.ExtractOne(userQuestion, _botData.Keywords);
            //  limit responses by keyword if found
            if (keywordMatch != null && keywordMatch.Score >= 60)
            {
                _currentTopic = keywordMatch.Value;

                if (userQuestion.Contains("tip"))
                {
                    answerString += GetRandomTip(userQuestion, _currentTopic);
                }
                else
                {
                    var keywordAnswers = Process.ExtractAll(_currentTopic, _botData.QnA.Keys)
                        .Where(question => question.Score > 75)
                        .Select(question => _botData.QnA[question.Value])
                        .ToList();
                    var choicesDict = _botData.QnA.Where(kvp => keywordAnswers.Contains(kvp.Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value); // Copilot
                    choicesDict.Add("", _botData.QnA[""]);

                    answerString += $"{GetBestMatch(userQuestion, choicesDict)}";
                }
            }
            else
            {
                answerString += $"{GetBestMatch(userQuestion, _botData.QnA)}";
            }

            return answerString;
        }

        //=========================================================//
        // Use the FuzzySharp package to return an answer based
        // on a similarity score, comparing the user question to
        // the questions in the QnA dictionary
        private string GetBestMatch(string userQuestion, Dictionary<string, string> qDict)
        {
            string answer = "";

            // record the user's interest if stated
            if (userQuestion.Contains("interested"))
            {
                _userInterest = _currentTopic;
                answer += $"{_botData.GetInterestOpener(_userInterest)}";
            }

            var prevSentiment = _currentSentiment;

            SetSentiment(userQuestion);

            // does the user appear confused by the previous answer?
            if ((prevSentiment != _currentSentiment) && (_currentSentiment == (int)BotData.Sentiment.CONFUSED))
            {
                answer += (answer.Length > 0 ? "\n" : "") + "I understand, it can be confusing. Let me try to explain further.";
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
                if (_userInterest != null && userQuestion.Contains(_userInterest) && _inputHistory.Count() > 2)
                {
                    answer += (answer.Length > 0 ? "\n" : "") + $"{_botData.RecallInterest(_userInterest)[_rand.Next(0, 8)]}";
                }
                else
                {
                    answer = (answer.Length > 0 ? "" : _botData.Openers[_currentSentiment][_rand.Next(0, _botData.Openers[_currentSentiment].Count())]) + $"\n{answer}";
                }
                // Add answer
                answer += (answer.Length > 0 ? "\n" : "") + $"{qDict[bestMatch]}";
                // Add follow up
                answer += $"\n{GetFollowUp("")}";
            }
            else
            {
                // Default response
                answer += (answer.Length > 0 ? "\n" : "") + $"{qDict[bestMatch]}";
            }
            _answerHistory.Add(qDict[bestMatch]);

            return answer;
        }


        //=========================================================//
        // Returns a random tip given a topic
        private string GetRandomTip(string userQuestion, string keyword)
        {
            string answer = "";
            userQuestion = userQuestion.ToLower();

            SetSentiment(userQuestion);

            // Add an opener
            answer += GetSentimentOpener();

            // Add tip if not the same as previous output
            int rand;
            string tip = "";
            do
            {
                rand = _rand.Next(0, _botData.Tips[$"{keyword} tip"].Count());
                tip = _botData.Tips[$"{keyword} tip"][rand];
                if (_answerHistory.Count > 0 && tip != _answerHistory.Last())
                {
                    answer +=$"\n{tip}";
                }
            } while (_answerHistory.Count > 0 && tip.Equals(_answerHistory.Last()));

            _answerHistory.Add(tip);

            // Add follow up
            answer += $"\n{GetFollowUp(keyword)}";

            return answer;
        }


        //=========================================================//
        // Returns a sentiment based opener
        private string GetSentimentOpener()
        {
            return _botData.Openers[_currentSentiment][_rand.Next(0, _botData.Openers[_currentSentiment].Count())];
        }


        //=========================================================//
        // Returns a random follow-up response
        private string GetFollowUp(string topic)
        {
            if (topic.Length == 0)
            {
                return _botData.FollowUps[_rand.Next(0, _botData.FollowUps.Count())];
            }
            else
            {
                return _botData.GetFollowUps(topic)[_rand.Next(0, _botData.FollowUps.Count())];
            }
        }


        //=========================================================//
        // Sets the current sentiment based on words contained in
        // the latest user input
        private void SetSentiment(string question)
        {
            // Default mood
            _currentSentiment = (int)BotData.Sentiment.NEUTRAL;
            foreach (string word in question.Split(' '))
            {
                if (word.Length > 3)
                {
                    if (Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.WORRIED]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.WORRIED;
                    }
                    else if (Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.CURIOUS]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.CURIOUS;
                    }
                    else if (Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.HAPPY]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.HAPPY;
                    }
                    else if (Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.FRUSTRATED]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.FRUSTRATED;
                    }
                    else if (Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.CONFUSED]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.CONFUSED;
                    }
                }
            }
        }

        public string Greeting()
        {
            return $"Welcome back {_botData.UserName}. {_botData.FirstMessageEndings[_rand.Next(0, _botData.FirstMessageEndings.Count)]}";
        }

        private void AddTask(string userQuestion)
        {
            _botData.Tasks.Add(new TaskReminder("Placeholder", "Placeholder", DateTime.Now));
            _botData.UpdateTasks();
        }

        private void DeleteTask(string userQuestion)
        {

        }

    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\