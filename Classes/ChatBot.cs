// References
// https://www.csharptutorial.net/csharp-regular-expression/csharp-regex/
// https://www.csharptutorial.net/csharp-regular-expression/csharp-regex-alternation/
// https://www.csharptutorial.net/csharp-regular-expression/csharp-regex-lookahead/
// https://www.csharptutorial.net/csharp-regular-expression/regex-non-capturing-group/
// https://chatgpt.com
// https://github.com/JakeBayer/FuzzySharp
// https://regex101.com/

using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using System;
using System.Collections.Generic;
using System.Data;
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
        private List<string> _activityLog = new List<string>();

        private List<string> _stopWords = new List<string>
        {
            "a", "about", "actually", "almost", "also", "although", "always", "am", "an", "and", "any", "are", "as", "at",
            "be", "became", "become", "but", "by", "can", "could", "did", "do", "does", "each", "either", "else", "for",
            "from", "had", "has", "have", "hence", "how", "i", "if", "in", "is", "it", "its", "just", "may", "maybe",
            "me", "might", "mine", "must", "my", "neither", "nor", "not", "of", "oh", "ok", "the", "then", "they", "when", "where", 
            "whereas", "wherever", "whenever", "whether", "which", "while", "who", "whom", "whoever", "whose", "why", "will",
            "with", "within", "without", "would", "yes", "yet", "you", "your"
        };

        private List<string> _createTaskKeywords = new List<string> { "add", "create", "append", "make", "save", "remind me to" };
        private List<string> _readTaskKeywords = new List<string> { "show", "list", "view", "display", "output", "print" };
        private List<string> _updateTaskKeywords = new List<string> { "amend", "alter", "change", "update", "modify", "edit" };
        private List<string> _deleteTaskKeywords = new List<string> { "remove", "delete", "discard", "trash", "dispose" };

        private string _taskPattern = @"\b(?:add|create|append|make|save|show|list|view|display|output|give|amend|alter|change|update|modify|edit|remove|delete|discard|trash|dispose).*\b(?:tasks?|reminders?)\b|\b(remind me to)\b";
        private string _quizPattern = @"quiz|game";
        private string _activityPattern = @"(?=display|show|list|print)(?=.*activity|activities).*";

        private enum ChatBotState
        {
            IDLE,
            HANDLE_QUIZ,
            HANDLE_TASK,
            ACTIVITY_LOG
        }

        private enum TaskActionState
        {
            IDLE,
            CREATE,
            READ,
            UPDATE,
            DELETE
        }

        private enum CreateTaskState
        {
            IDLE,
            TITLE,
            REMINDER,
            REMINDER_RETRY,
            DESCRIPTION,
            CONFIRM
        }
        // for storing task data while it is being gathered
        private TaskReminder _tempTask;


        // initialise states
        private ChatBotState _currentState = ChatBotState.IDLE;
        private TaskActionState _currentTaskState = TaskActionState.IDLE;
        private CreateTaskState _createTaskState = CreateTaskState.IDLE;

        private Random _rand = new Random();

        //=========================================================//
        // Default Constructor
        public ChatBot(string userName)
        {
            // Initialise data
            _botData = new BotData(userName);
        }


        //=========================================================//
        // Return a response to the user's input after identifying
        // the user's intent
        public string ProcessInput(string userInput)
        {
            string response = "";
            // log input
            _inputHistory.Add(userInput);
            // pre-processing
            var processedInput = PreProcess(userInput);
           // intent recognition
            if (Regex.Match(userInput.Trim().ToLower(), _taskPattern).Success)
            {
                _currentState = ChatBotState.HANDLE_TASK;
            }
            else if (Regex.Match(processedInput, _quizPattern).Success)
            {
                _currentState = ChatBotState.HANDLE_QUIZ;
            }
            else if(Regex.Match(userInput.Trim().ToLower(), _activityPattern).Success)
            {
                _currentState = ChatBotState.ACTIVITY_LOG;
            }

            // take action based on state
            if (_currentState == ChatBotState.IDLE)
            {
                response = AnswerQuestion(processedInput);
            }
            else if (_currentState == ChatBotState.HANDLE_QUIZ)
            {
                response = HandleQuizQuery(userInput);
            }
            else if (_currentState == ChatBotState.HANDLE_TASK)
            {
                response = HandleTaskQuery(userInput);
            }
            else if (_currentState == ChatBotState.ACTIVITY_LOG)
            {
                response = DisplayActivityLog();
            }

            // log response
            _answerHistory.Add(response);

            return response;
        }


        //=================================== QUESTION ANSWERING ===================================//

        //=======================================================//
        // Return an answer, taking keywords and sentiment into
        // account.
        private string AnswerQuestion(string question)
        {
            List<string> answerLines = new List<string>();
            bool interestExpressed = false;

            // sentiment detection
            SetSentiment(question);

            // keyword recognition
            var keywordMatch = FuzzySharp.Process.ExtractOne(question, _botData.Keywords);
            if (keywordMatch != null && keywordMatch.Score >= 60)
            {
                answerLines.Add(GetKeywordAnswer(question, keywordMatch.Value));
                // record the user's interest if stated
                if (question.Contains("interest"))
                {
                    interestExpressed = true;
                    _userInterest = _currentTopic;
                }
            }
            else
            {
                keywordMatch = null;
                // general query
                answerLines.Add($"{GetBestMatch(question, _botData.QnA)}");
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

             return string.Join("\n", answerLines);
        }


        //===========================================================//
        // Return an answer relevant to a specific keyword
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
        // Returns a sentiment based opening line
        private string GetSentimentOpener()
        {
            return _botData.Openers[(int)_currentSentiment][_rand.Next(0, _botData.Openers[(int)_currentSentiment].Count())];
        }


        //=========================================================//
        // Returns a random follow-up response line
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


        //=================================== TASK METHODS ===================================//

        private string HandleTaskQuery(string userInput)
        {
            // determine intent if state is idle
            if (_currentTaskState == TaskActionState.IDLE)
            {
                if ((FuzzySharp.Process.ExtractOne(PreProcess(userInput), _createTaskKeywords, scorer: ScorerCache.Get<TokenSetScorer>()).Score >= 60))
                {
                    _currentTaskState = TaskActionState.CREATE;
                }
                else if ((FuzzySharp.Process.ExtractOne(PreProcess(userInput), _readTaskKeywords, scorer: ScorerCache.Get<TokenSetScorer>()).Score >= 60))
                {
                    _currentTaskState = TaskActionState.READ;
                }
                else if ((FuzzySharp.Process.ExtractOne(PreProcess(userInput), _updateTaskKeywords, scorer: ScorerCache.Get<TokenSetScorer>()).Score >= 60))
                {
                    _currentTaskState = TaskActionState.UPDATE;
                }
                else if ((FuzzySharp.Process.ExtractOne(PreProcess(userInput), _deleteTaskKeywords, scorer: ScorerCache.Get<TokenSetScorer>()).Score >= 60))
                {
                    _currentTaskState = TaskActionState.DELETE;
                }
            }
            // then
            switch (_currentTaskState)
            {
                case TaskActionState.CREATE:
                    return AddTask(userInput);
                case TaskActionState.READ:
                    return ViewTask(userInput);
                case TaskActionState.UPDATE:
                    return "update task";
                case TaskActionState.DELETE:
                    return "delete task";
                default:
                    return "No task action recognised.";
            }
        }


        //==============================================================//
        // goes through the process of collecting task details and saves
        // the new task
        private string AddTask(string userInput)
        {
            // set title
            if (_createTaskState == CreateTaskState.IDLE)
            {
                _tempTask = new TaskReminder();
                _tempTask.OriginalInput = userInput;
                userInput = userInput.Trim().ToLower();
                // set task details
                var titleSearch = Regex.Match(userInput, @"(?:remind\s+me\s+to\s+|task:|task \-|reminder:|reminder \-)(.*)");
                if (titleSearch.Success)
                {
                    if (titleSearch.Groups.Count > 1)
                    {
                        _tempTask.Title = titleSearch.Groups[1].Value.Trim();
                        _createTaskState = CreateTaskState.REMINDER;
                    }
                }
                else
                {
                    _createTaskState = CreateTaskState.TITLE;
                    return "I'm on it! please enter the task title";
                }
            }
            
            if (_createTaskState == CreateTaskState.TITLE)
            {
                _tempTask.Title = userInput;
                _createTaskState = CreateTaskState.REMINDER;
            }
            // set reminder
            if (_createTaskState == CreateTaskState.REMINDER)
            {
                if (GetTime(userInput) == false)
                {
                    _createTaskState = CreateTaskState.REMINDER_RETRY;
                    return "Do you want a reminder?\nIf you do, please enter \"Yes\", followed by a time in the format [number] [unit] from now\nFor example: 15 minutes from now";
                }
                else
                {
                    _createTaskState = CreateTaskState.DESCRIPTION;
                }
            }

            if (_createTaskState == CreateTaskState.REMINDER_RETRY)
            {
                if (userInput.Trim().ToLower().StartsWith("yes"))
                {
                    if (GetTime(userInput) == false)
                    {
                        return "Something isn't quite right. Please check that the format is like \" 5 days from now\" and try again.";
                    }
                }
                _createTaskState = CreateTaskState.DESCRIPTION;
            }
            // set description
            if (_createTaskState == CreateTaskState.DESCRIPTION)
            {
                if (_tempTask.Reminder != new DateTime())
                {
                    _tempTask.Description = $"Task: \'{_tempTask.Title}\', Reminder: {_tempTask.Reminder.ToString()}";
                }
                else
                {
                    _tempTask.Description = $"Task \'{_tempTask.Title}\', Reminder: none";
                }
                _createTaskState = CreateTaskState.CONFIRM;
            }
            // save task
            if (_createTaskState == CreateTaskState.CONFIRM)
            {
                _botData.Tasks.Add(_tempTask);
                _botData.UpdateTasks();
                // return state to idle
                _createTaskState = CreateTaskState.IDLE;
                _currentState = ChatBotState.IDLE;
                _activityLog.Add($"Task created: {_tempTask.Description}.");
                return $"Task created: {_tempTask.Description}.\nIf I can help in any other way, please don't hesitate to ask.";
            }
            return "Unfortunately I am unable to add tasks at this time. Please try again at a later date.";
        }

        private string ViewTask(string userInput)
        {

            // If no tasks exist
            if (_botData.Tasks == null || _botData.Tasks.Count == 0)
            {
                return "You have no tasks saved.\nYou can add a task by saying \"Remind me to ABC XY minutes from now\" (time is optional)";
            }

            // If no specific match, list all tasks
            string allTasks = "Here are your current tasks:\n";
            foreach (TaskReminder task in _botData.Tasks)
            {
                allTasks += $"- {task.Title}, Reminder: {(task.Reminder != DateTime.MinValue ? task.Reminder.ToString() : "none")}\n";
            }
            _activityLog.Add("Displayed existing tasks.");
            _currentState = ChatBotState.IDLE;
            return allTasks + GetFollowUp("");
        }

        private void UpdateTask(string userInput)
        {

        }

        private void DeleteTask(string userInput)
        {

        }


        //=================================== QUIZ METHODS ===================================//

        private string HandleQuizQuery(string userInput)
        {
            string response = "quiz time!";
            return response;
        }


        //=================================== ACTIVITY LOG METHODS ===================================//

        private string DisplayActivityLog()
        {
            if (_activityLog.Count == 0)
            {
                return "No activities have been logged yet.\nActivities are logged when important events take place.\n" + GetFollowUp("");
            }

            string response = "";
            var pos = 1;
            // reverse the activity log
            _activityLog.Reverse();
            foreach(var log in _activityLog.Take(10))
            {                
                response += string.Format("{0, -2}. {1}\n", pos, log);
                pos++;
            }
            // reorder acitivity log
            _activityLog.Reverse();
            _currentState = ChatBotState.IDLE;
            return "Sure thing!\nHere are the recently logged activities.\n" + response + GetFollowUp("");
        }

        //======================== HELPER METHODS ========================//

        //=======================================================//
        // Apply NLP pre-processing to a string
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

        //====================================================//
        // tries to return a time based on a string
        private bool GetTime(string userInput)
        {
            var timePattern = @"(\d+)\s+(year|month|week|day|hour|minute|second)s?\s+(from|after)\s+(now|today|tomorrow)";
            var timeRegex = Regex.Match(userInput, timePattern);
            if(timeRegex.Success)
            {
                var quantity = int.Parse(timeRegex.Groups[1].Value);
                var unit = timeRegex.Groups[2].Value;
                switch (timeRegex.Groups[4].Value)
                {
                    case "now":
                        _tempTask.Reminder = SetTime(DateTime.Now, quantity, unit);
                        return true;
                    case "today":
                        _tempTask.Reminder = SetTime(DateTime.Today, quantity, unit);
                        return true;
                    case "tomorrow":
                        _tempTask.Reminder = SetTime(DateTime.Now.AddDays(1), quantity, unit);
                        return true;
                    default:
                        _tempTask.Reminder = SetTime(DateTime.Now, quantity, unit);
                        return true;
                }
            }
            return false;
        }


        private DateTime SetTime(DateTime start, int qty, string unit)
        {
            // force singular
            if (unit.EndsWith("s"))
            {
                unit = unit.Substring(0, unit.Length - 1);
            }

            switch (unit)
            {
                case "second":
                    return start.AddSeconds(qty);
                case "minute":
                    return start.AddMinutes(qty);
                case "hour":
                    return start.AddHours(qty);
                case "day":
                    return start.AddDays(qty);
                case "week":
                    return start.AddDays(qty * 7);
                case "month":
                    return start.AddMonths(qty);
                case "year":
                    return start.AddYears(qty);
                    default: return start.AddHours(1);
            }
        }

        //====================================================//
        // Return a random greeting for when the user opens
        // the chat
        public string Greeting()
        {
            return $"Welcome back {_botData.UserName}. {_botData.FirstMessageEndings[_rand.Next(0, _botData.FirstMessageEndings.Count)]}";
        }
    }
}
////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\