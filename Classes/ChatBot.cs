// References
// https://chatgpt.com
// https://github.com/JakeBayer/FuzzySharp

using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ST10445832_PROG6221_PoE.Classes
{
    public class ChatBot
    {
        private BotData _botData;

        private int? _currentSentiment;
        private int? _prevSentiment;
        private string _userInterest;
        private string _currentTopic;
        private List<string> _inputHistory = new List<string>();
        private List<string> _answerHistory = new List<string>();

        private List<string> _stopWords = new List<string>
        {
            "a", "about", "actually", "almost", "also", "although", "always", "am", "an", "and", "any", "are", "as", "at",
            "be", "became", "become", "but", "by", "can", "could", "did", "do", "does", "each", "either", "else", "for",
            "from", "had", "has", "have", "hence", "how", "i", "if", "in", "is", "it", "its", "just", "may", "maybe",
            "me", "might", "mine", "must", "my", "neither", "nor", "not", "of", "oh", "ok", "the", "then", "they", "when", "where", 
            "whereas", "wherever", "whenever", "whether", "which", "while", "who", "whom", "whoever", "whose", "why", "will",
            "with", "within", "without", "would", "yes", "yet", "you", "your"
        };

        private string TaskPattern = @"";
        private string QuizPattern = @"";

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
            bool interestExpressed = false;
            List<string> answerLines = new List<string>();

            // log input
            _inputHistory.Add(userQuestion);
            // pre-processing
            userQuestion = PreProcess(userQuestion);
            // sentiment detection
            SetSentiment(userQuestion);

            // intent recognition
            /// task
            //// task regex
            if (Regex.Match(userQuestion, TaskPattern).Success)
            {
                // check task action
                /// add
                AddTask(userQuestion, userQuestion.Contains("remind"));
                /// view
                /// update
                /// delete
            }

            // quiz
            // quiz regex
            if (Regex.Match(userQuestion, QuizPattern).Success)
            {
                // start quiz

            }


            // keyword recognition
            var keywordMatch = FuzzySharp.Process.ExtractOne(userQuestion, _botData.Keywords);
            Debug.WriteLine($"Keyword:{keywordMatch.Value} {keywordMatch.Score}");
            if (keywordMatch != null && keywordMatch.Score >= 60)
            {
                answerLines.Add(GetKeywordAnswer(userQuestion, keywordMatch.Value));
                // record the user's interest if stated
                if (userQuestion.Contains("interest"))
                {
                    interestExpressed = true;
                    _userInterest = _currentTopic;
                }
            }
            else
            {
                keywordMatch = null;
                // general query
                answerLines.Add($"{GetBestMatch(userQuestion, _botData.QnA)}");
            }

            // add opener to answer
            if (interestExpressed)
            {
                answerLines.Insert(0, _botData.GetInterestOpener(_userInterest));
            }
            else if (_userInterest != null && keywordMatch != null && keywordMatch.Value == _userInterest)
            {
                answerLines.Insert(0, _botData.RecallInterest(_userInterest)[_rand.Next(0, 8)]);
            }
            else
            {
                answerLines.Insert(0, GetSentimentOpener());
            }

            // if user is following up and seems confused
            if ((_prevSentiment != _currentSentiment) && keywordMatch != null && (_currentSentiment == (int)BotData.Sentiment.CONFUSED))
            {
                answerLines.Insert(0, "I understand, it can be confusing. Let me try to explain further.");
            }

            // add answer ending
            answerLines.Add(GetFollowUp((keywordMatch != null) ? keywordMatch.Value : ""));
            // assemble answer string
            string answer = string.Join("\n", answerLines);
            // log answer
            _answerHistory.Add(answer);

            return answer;
        }


        private string PreProcess(string question)
        {
            // trim & lower case
            question = question.Trim().ToLower();
            // remove punctuation
            question = new string(question.Where(ch => !char.IsPunctuation(ch)).ToArray());
            // remove stop words
            var tokenised = question.Split(' ');
            var noStop = tokenised.Where(word => !_stopWords.Contains(word));
            question = string.Join(" ", noStop);
            return question;
        }


        private string GetKeywordAnswer(string question, string keyword)
        {
            _currentTopic = keyword;

            if (question.Contains("tip"))
            {
                return GetRandomTip(question, _currentTopic);
            }
            else
            {
                // limit possible answers according to the keyword
                var keywordAnswers = FuzzySharp.Process.ExtractAll(_currentTopic, _botData.QnA.Keys)
                    .Where(q => q.Score > 75)
                    .Select(q => _botData.QnA[q.Value])
                    .ToList();
                var choicesDict = _botData.QnA.Where(kvp => keywordAnswers.Contains(kvp.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value); // Copilot
                choicesDict.Add("", _botData.QnA[""]);

                return $"{GetBestMatch(question, choicesDict)}";
            }
        }


        //=========================================================//
        // Use the FuzzySharp package to return an answer based
        // on a similarity score, comparing the user question to
        // the questions in the provided
        private string GetBestMatch(string userQuestion, Dictionary<string, string> qDict)
        {
            // best match score
            int fuzzMax = 0;
            // best match question
            string bestMatch = "";
            foreach (string question in qDict.Keys)
            {
                var q = PreProcess(question);
                // compare the question to questions which have answers, ignoring the order of the words in the question
                var score = Fuzz.TokenSetRatio(userQuestion, q);
                if (score > 60 && score > fuzzMax)
                {
                    fuzzMax = score;
                    bestMatch = question;
                }
            }

            return qDict[bestMatch];
        }


        //=========================================================//
        // Returns a random tip given a topic
        private string GetRandomTip(string userQuestion, string keyword)
        {
            string answer = "";

            // Add tip if not the same as previous output
            int rand;
            string tip = "";
            do
            {
                rand = _rand.Next(0, _botData.Tips[$"{keyword} tip"].Count());
                tip = _botData.Tips[$"{keyword} tip"][rand];
                if (_answerHistory.Count > 0 && tip != _answerHistory.Last())
                {
                    answer += tip;
                }
            } while (_answerHistory.Count > 0 && tip.Equals(_answerHistory.Last()));

            return answer;
        }


        //=========================================================//
        // Returns a sentiment based opener
        private string GetSentimentOpener()
        {
            return _botData.Openers[(int)_currentSentiment][_rand.Next(0, _botData.Openers[(int)_currentSentiment].Count())];
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
            if (_currentSentiment != null)
            {
                _prevSentiment = _currentSentiment;
            }

            // Default mood
            _currentSentiment = (int)BotData.Sentiment.NEUTRAL;
            foreach (string word in question.Split(' '))
            {
                if (word.Length > 3)
                {
                    if (FuzzySharp.Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.WORRIED]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.WORRIED;
                    }
                    else if (FuzzySharp.Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.CURIOUS]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.CURIOUS;
                    }
                    else if (FuzzySharp.Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.HAPPY]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.HAPPY;
                    }
                    else if (FuzzySharp.Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.FRUSTRATED]).Score >= 60)
                    {
                        _currentSentiment = (int)BotData.Sentiment.FRUSTRATED;
                    }
                    else if (FuzzySharp.Process.ExtractOne(word, _botData.SentimentWords[(int)BotData.Sentiment.CONFUSED]).Score >= 60)
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

        private void AddTask(string userQuestion, bool reminder)
        {
            // request task details
                // title
                // description
                // if reminder true ask when
                // else ask if reminder wanted
            // save task
            _botData.Tasks.Add(new TaskReminder("Placeholder", "Placeholder", DateTime.Now));
            _botData.UpdateTasks();
        }

        private void ViewTask(string userInput)
        {

        }

        private void UpdateTask(string userInput)
        {

        }

        private void DeleteTask(string userInput)
        {

        }

    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\