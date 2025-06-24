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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace ST10445832_PROG6221_PoE.Classes
{
    public class ChatBot
    {
        public BotData _botData;

        private int? _currentSentiment;
        private int? _prevSentiment;
        private string _userInterest;
        private string _currentTopic;
        private List<string> _inputHistory = new List<string>();
        private List<string> _answerHistory = new List<string>();
        private List<string> _activityLog = new List<string>();

        private List<string> _createTaskKeywords = new List<string> { "add", "create", "append", "make", "save", "remind me to" };
        private List<string> _readTaskKeywords = new List<string> { "show", "list", "view", "display", "output", "print" };
        private List<string> _updateTaskKeywords = new List<string> { "amend", "alter", "change", "update", "modify", "edit" };
        private List<string> _deleteTaskKeywords = new List<string> { "remove", "delete", "discard", "trash", "dispose" };

        private string _taskPattern = @"\b(?:add|create|append|make|save|show|list|view|display|output|give|amend|alter|change|update|modify|edit|remove|delete|discard|trash|dispose).*\b(?:tasks?|reminders?)\b|\b(remind me to)\b";
        private string _quizPattern = @"quiz|game|mini-game";
        private string _activityPattern = @"(?=display|show|list|print)(?=.*activity|activities).*";

        // for tracking which mode the chatbot is in
        private enum ChatBotState
        {
            IDLE,
            HANDLE_QUIZ,
            HANDLE_TASK,
            ACTIVITY_LOG
        }
        // for tracking which task action is active
        private enum TaskActionState
        {
            IDLE,
            CREATE,
            READ,
            UPDATE,
            DELETE
        }
        // for tracking the steps of creating a new task
        private enum CreateTaskState
        {
            IDLE,
            TITLE,
            REMINDER,
            REMINDER_RETRY,
            DESCRIPTION,
            CONFIRM
        }
        // for tracking the steps of updating a new task
        private enum UpdateTaskState
        {
            IDLE,
            SELECTED,
            UPDATE_TITLE,
            UPDATE_REMINDER,
            CONFIRM
        }
        // for tracking the steps of updating a new task
        private enum DeleteTaskState
        {
            IDLE,
            SELECTED,
            CONFIRM
        }
        // for tracking quiz state
        private enum QuizState
        {
            IDLE,
            QUESTION,
            ANSWER,
            FEEDBACK
        }
        // initialise states
        private ChatBotState _currentChatbotState = ChatBotState.IDLE;
        private TaskActionState _currentTaskState = TaskActionState.IDLE;
        private CreateTaskState _createTaskState = CreateTaskState.IDLE;
        private UpdateTaskState _updateTaskState = UpdateTaskState.IDLE;
        private DeleteTaskState _deleteTaskState = DeleteTaskState.IDLE;
        private QuizState _quizState = QuizState.IDLE;
        // for storing task data while it is being gathered
        private TaskReminder _tempTask;

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
                _currentChatbotState = ChatBotState.HANDLE_TASK;
            }
            else if (Regex.Match(processedInput, _quizPattern).Success)
            {
                _currentChatbotState = ChatBotState.HANDLE_QUIZ;
            }
            else if (Regex.Match(userInput.Trim().ToLower(), _activityPattern).Success)
            {
                _currentChatbotState = ChatBotState.ACTIVITY_LOG;
            }

            // take action based on state
            if (_currentChatbotState == ChatBotState.IDLE)
            {
                response = AnswerQuestion(processedInput);
            }
            else if (_currentChatbotState == ChatBotState.HANDLE_QUIZ)
            {
                response = HandleQuizQuery(userInput);
            }
            else if (_currentChatbotState == ChatBotState.HANDLE_TASK)
            {
                response = HandleTaskQuery(userInput);
            }
            else if (_currentChatbotState == ChatBotState.ACTIVITY_LOG)
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

            // check for detail request
            bool detailRequested = DetailRequested(question);

            if (detailRequested && !string.IsNullOrEmpty(_currentTopic))
            {
                string detail = _botData.GetDetail(_currentTopic);
                if (!string.IsNullOrEmpty(detail))
                {
                    answerLines.Add(detail);
                    var opener = _botData.GetDetailOpener(_currentTopic);
                    answerLines.Insert(0, opener);
                    answerLines.Add(GetFollowUp(_currentTopic));
                }
                else
                {
                    answerLines.Add($"I'm sorry {_botData.UserName}, unfortunately I cannot provide more detail on that subject.");
                    answerLines.Add(GetFollowUp(""));
                }
                return string.Join("\n", answerLines);
            }

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


        //==========================================================//
        // Returns true if a request for more detail is detected
        private bool DetailRequested(string question)
        {
            List<string> detailPhrases = new List<string>
            {
                "more detail",
                "explain",
                "tell me more",
                "clarify",
                "elaborate",
                "go deeper"
            };

            foreach (string phrase in detailPhrases)
            {
                int score = Fuzz.PartialRatio(question, phrase);
                if (score > 75)
                {
                    return true;
                }
            }

            return false;
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
                    return UpdateTask(userInput);
                case TaskActionState.DELETE:
                    return DeleteTask(userInput);
                default:
                    return "No task action recognised.";
            }
        }


        //==============================================================//
        // Handles the process of collecting task details and saves
        // the new task
        private string AddTask(string userInput)
        {
            // allow the user to terminate the process
            if (PreProcess(userInput).Equals("cancel"))
            {
                _createTaskState = CreateTaskState.IDLE;
                _currentTaskState = TaskActionState.IDLE;
                _currentChatbotState = ChatBotState.IDLE;
                return "Alright. I have stopped with the creation of a new task.\n" + GetFollowUp("");
            }

            // set title
            if (_createTaskState == CreateTaskState.IDLE)
            {
                _tempTask = new TaskReminder();
                _tempTask.Completed = false;
                _tempTask.OriginalInput = userInput;
                userInput = userInput.Trim().ToLower();
                // set task details
                var titleSearch = Regex.Match(userInput, @"(?:remind\s+me\s+to\s+|task:|task \-|reminder:|reminder \-)(.*?) (\d+) (seconds?|minutes?|hours?|days?|months?|years?)");

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
                if (_tempTask.Reminder != DateTime.MinValue)
                {
                    _tempTask.Description = $"Task: \'{_tempTask.Title}\', Reminder: {_tempTask.Reminder.ToString()}";
                }
                else
                {
                    _tempTask.Description = $"Task: \'{_tempTask.Title}\', Reminder: none";
                }
                _createTaskState = CreateTaskState.CONFIRM;
            }
            // save task
            if (_createTaskState == CreateTaskState.CONFIRM)
            {
                try
                {
                    _tempTask.TaskNumber = _botData.Tasks.Last().TaskNumber + 1;
                }
                catch (InvalidOperationException)
                {
                    _tempTask.TaskNumber = 1;
                }
                _botData.Tasks.Add(_tempTask);
                _botData.UpdateTasks();
                // return state to idle
                _createTaskState = CreateTaskState.IDLE;  // reset task creation state
                _currentTaskState = TaskActionState.IDLE; // reset CRUD action
                _currentChatbotState = ChatBotState.IDLE; // reset bot mode
                _activityLog.Add($"Task created: {_tempTask.Description}.");
                return $"Task created!\n{_tempTask.Description}.\nIf I can help in any other way, please don't hesitate to ask.";
            }

            return $"Sorry {_botData.UserName}. Unfortunately I am unable to add tasks at this time. Please try again at a later date.\n" + GetFollowUp("");
        }


        //=========================================================//
        // Returns a string containing all existing tasks (if there
        // are any)
        private string ViewTask(string userInput)
        {

            // let the user know if there are no tasks
            if (_botData.Tasks == null || _botData.Tasks.Count == 0)
            {
                _currentChatbotState = ChatBotState.IDLE;
                _currentTaskState = TaskActionState.IDLE;
                return "You have no tasks saved.\nYou can add a task by saying \"Remind me to <Task Name> <Integer> <Unit of time> from now\" (time is optional)";
            }

            // list all tasks
            string allTasks = "Certainly!\nHere are your current tasks:\n\n";
            foreach (TaskReminder task in _botData.Tasks.Where(t => t.Completed == false).ToList())
            {
                allTasks += $"Task Number: {task.TaskNumber}\nTitle: {task.Title}\nReminder: {(task.Reminder != DateTime.MinValue ? task.Reminder.ToString() : "none")}\n" +
                    $"Status: {(task.Completed ? "Completed" : "Active")}\n\n";
            }
            _activityLog.Add("Displayed existing tasks.");
            _currentChatbotState = ChatBotState.IDLE;
            _currentTaskState = TaskActionState.IDLE;
            return allTasks + GetFollowUp("");
        }


        //=======================================================//
        // Handles the process of updating a task
        private string UpdateTask(string userInput)
        {
            // let the user know if there are no tasks
            if (_botData.Tasks == null || _botData.Tasks.Count == 0)
            {
                return "You have no tasks saved.\nYou can add a task by saying \"Remind me to ABC XY minutes from now\" (time is optional)";
            }

            // allow the user to terminate the process
            if (PreProcess(userInput).Equals("cancel"))
            {
                _updateTaskState = UpdateTaskState.IDLE;
                _currentTaskState = TaskActionState.IDLE;
                _currentChatbotState = ChatBotState.IDLE;
                return "Alright. I have stopped with the creation of a new task.\n" + GetFollowUp("");
            }

            // look for the task
            if (_updateTaskState == UpdateTaskState.IDLE)
            {
                var taskNumberRegex = Regex.Match(PreProcess(userInput), @"\d+");
                if (taskNumberRegex.Success)
                {
                    var results = _botData.Tasks.Where(task => task.TaskNumber == int.Parse(taskNumberRegex.Value)).ToList();

                    if (results.Count == 1)
                    {
                        _tempTask = results[0];
                        _updateTaskState = UpdateTaskState.SELECTED;
                        return $"Perfect! I have found task {_tempTask.TaskNumber}.\nWould you like to update the title, reminder, or mark as completed?";
                    }
                    else
                    {
                        return $"Hmm. I was unable to find any task with the task number \"{taskNumberRegex.Value}\".\nPlease double-check the task number and try again.";
                    }
                }
                else
                {
                    return "Please enter the task number of the task you want to update.";
                }
            }

            // request which field to update
            if (_updateTaskState == UpdateTaskState.SELECTED)
            {
                var taskFieldUpdateRegex = Regex.Match(PreProcess(userInput), @"\btitle\b|\breminder\b");
                if (taskFieldUpdateRegex.Success && taskFieldUpdateRegex.Value.Trim().Equals("title"))
                {
                    _updateTaskState = UpdateTaskState.UPDATE_TITLE;
                    return "Let's get that title changed!\nWhat would you like the new title to be?";
                }
                else if (taskFieldUpdateRegex.Success && taskFieldUpdateRegex.Value.Trim().Equals("reminder"))
                {
                    _updateTaskState = UpdateTaskState.UPDATE_REMINDER;
                    return "Let's update that reminder!\nWhen would you like to set the reminder for?\n";
                }
                else if (Regex.Match(PreProcess(userInput), @"\bmark\b .* \bcompleted?\b").Success)
                {
                    _updateTaskState = UpdateTaskState.CONFIRM;
                    _tempTask.Completed = true;
                }
                else
                {
                    return $"I'm sorry {_botData.UserName}, I didn't quite catch that. Did you want to change the title or the reminder?";
                }

            }

            // update the selected field
            if (_updateTaskState == UpdateTaskState.UPDATE_TITLE)
            {
                _tempTask.Title = userInput;
                _updateTaskState = UpdateTaskState.CONFIRM;
            }

            if (_updateTaskState == UpdateTaskState.UPDATE_REMINDER)
            {
                if (!GetTime(userInput))
                {
                    return $"Sorry {_botData.UserName}. Unfortunately I don't understand when you want to be reminded.\n Please enter re-enter and make sure to " +
                        $"follow the correct format; eg. \"2 hours from now\"";
                }
                else
                {
                    _updateTaskState = UpdateTaskState.CONFIRM;
                }
            }

            // save the changes
            if (_updateTaskState == UpdateTaskState.CONFIRM)
            {
                var indexToUpdate = _botData.Tasks.IndexOf(_botData.Tasks.Where(task => task.TaskNumber == _tempTask.TaskNumber).First());
                _botData.Tasks[indexToUpdate] = _tempTask;
                _updateTaskState = UpdateTaskState.IDLE;
                _currentTaskState = TaskActionState.IDLE;
                _currentChatbotState = ChatBotState.IDLE;
                _botData.UpdateTasks();
                return $"All done. I have updated task {_tempTask.TaskNumber} as you have requested.\n" + GetFollowUp("");
            }

            return "Oh dear. Something has gone horribly wrong and I was unable to carry out your request.\nPlease accept my apologies\n" + GetFollowUp("");
        }


        //=======================================================//
        // Handles the process of deleting a task
        private string DeleteTask(string userInput)
        {
            // let the user know if there are no tasks
            if (_botData.Tasks == null || _botData.Tasks.Count == 0)
            {
                _currentChatbotState = ChatBotState.IDLE;
                _currentTaskState = TaskActionState.IDLE;
                return "You have no tasks saved.\nYou can add a task by saying \"Remind me to ABC XY minutes from now\" (time is optional)";
            }

            // allow the user to terminate the process
            if (PreProcess(userInput).Equals("cancel"))
            {
                _deleteTaskState = DeleteTaskState.IDLE;
                _currentTaskState = TaskActionState.IDLE;
                _currentChatbotState = ChatBotState.IDLE;
                return "Alright. I have stopped with the deletion of the task.\n" + GetFollowUp("");
            }

            // try to find task
            if (_deleteTaskState == DeleteTaskState.IDLE)
            {
                var taskNumberRegex = Regex.Match(PreProcess(userInput), @"\d+");
                if (taskNumberRegex.Success)
                {
                    var results = _botData.Tasks.Where(task => task.TaskNumber == int.Parse(taskNumberRegex.Value)).ToList();
                    // request delete confirmation
                    if (results.Count == 1)
                    {
                        _tempTask = results[0];
                        _deleteTaskState = DeleteTaskState.SELECTED;
                        return $"You have asked me to delete task {_tempTask.TaskNumber}.\n" +
                            $"Title: {_tempTask.Title}\nReminder: {_tempTask.Reminder}.\n\n" +
                            "Please confirm (yes/no)";
                    }
                    else
                    {
                        return $"Hmm. I was unable to find any task with the task number \"{taskNumberRegex.Value}\".\nPlease double-check the task number and try again.";
                    }
                }
                else
                {
                    return "Please enter the task number of the task you want to delete.";
                }
            }
            // check for confirmation
            if (_deleteTaskState == DeleteTaskState.SELECTED)
            {
                var confirmDeleteRegex = Regex.Match(PreProcess(userInput), @"\byes\b|\bno\b");
                if (confirmDeleteRegex.Success && confirmDeleteRegex.Value.Trim().Equals("yes"))
                {
                    _deleteTaskState = DeleteTaskState.CONFIRM;
                }
                else if (confirmDeleteRegex.Success && confirmDeleteRegex.Value.Trim().Equals("no"))
                {
                    _deleteTaskState = DeleteTaskState.IDLE;
                    return $"That's no problem. I won't delete task {_tempTask.TaskNumber}.\n" + GetFollowUp("");
                }
                else
                {
                    return $"Sorry {_botData.UserName}, I don't understand your response.\n Would you like to continue and delete task {_tempTask.TaskNumber}? (yes/no)";
                }
            }
            // try to delete task
            if (_deleteTaskState == DeleteTaskState.CONFIRM)
            {
                if (_botData.Tasks.Remove(_botData.Tasks.Where(task => task.TaskNumber == _tempTask.TaskNumber).First()))
                {
                    _deleteTaskState = DeleteTaskState.IDLE;
                    _currentTaskState = TaskActionState.IDLE;
                    _currentChatbotState = ChatBotState.IDLE;
                    _botData.UpdateTasks();
                    return $"Task {_tempTask.TaskNumber} has been successfully deleted!\n" + GetFollowUp("");
                }
                else
                {
                    return $"Oops. Something went wrong when I tried to delete task {_tempTask.TaskNumber}.\n" +
                        "I have aborted the process.\n" + GetFollowUp("");
                }
            }
            return "Oh dear. Something has gone horribly wrong and I was unable to carry out your request.\nPlease accept my apologies\n" + GetFollowUp("");
        }


        //=================================== QUIZ METHODS ===================================//

        //=================================================//
        // routing based on current quiz state
        private string HandleQuizQuery(string userInput)
        {
            // allow the user to end the quiz
            if (PreProcess(userInput).Equals("cancel"))
            {
                _currentChatbotState = ChatBotState.IDLE;
                _quizState = QuizState.IDLE;
                _activityLog.Add($"Quiz aborted on question {_botData.QuestionCounter + 1}");
                _botData.CorrectAnswers = 0;
                _botData.QuestionCounter = 0;
                return "Alright. I have terminated this round of the quiz.\n" + GetFollowUp("");
            }
            switch (_quizState)
            {
                case QuizState.IDLE:
                    return QuizIntro();
                case QuizState.QUESTION:
                    return AskQuestion();
                case QuizState.ANSWER:
                    return CheckAnswer(userInput);
                case QuizState.FEEDBACK:
                    return EndQuiz();
                default:
                    return "No task action recognised.";
            }
        }


        //==================================================//
        // Initialises Quiz questions and explains the rules
        private string QuizIntro()
        {
            _botData.SetRoundQuestions();
            _quizState = QuizState.QUESTION;
            return "So you want to test your knowledge of cybersecurity? Excellent!\n\n" +
                "You will be asked 10 questions - multiple choice and true/false.\n" +
                "To answer a multiple choice question, enter the letter which corresponds with" +
                " the answer you wish to select (eg. A).\n" +
                "To answer a true/false question, enter either \"true\" or \"false\".\n\n" +
                "Let me know when you are ready to begin!";
        }


        //==================================================================//
        // Reutrns the next quiz question in the correct format for its type
        private string AskQuestion()
        {

            var question = _botData.RoundQuestions[_botData.QuestionCounter];
            string output = "";
            if (question.GetType() == typeof(MultipleChoiceQuestion))
            {
                var q = (MultipleChoiceQuestion)question;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Question: {q.Question}");
                sb.AppendLine($"A) {q.Choices[0]}");
                sb.AppendLine($"B) {q.Choices[1]}");
                sb.AppendLine($"C) {q.Choices[2]}");
                sb.AppendLine($"D) {q.Choices[3]}");
                output = sb.ToString();
            }
            else
            {
                var q = (BoolQuestion)question;
                output = $"True or False: {q.Question}";
            }
            _quizState = QuizState.ANSWER;
            return output;
        }


        //==================================================================//
        // Checks the user's quiz question answer and returns feedback
        private string CheckAnswer(string userInput)
        {
            var question = _botData.RoundQuestions[_botData.QuestionCounter];
            var output = "";
            if (question.GetType() == typeof(MultipleChoiceQuestion))
            {
                var q = (MultipleChoiceQuestion)question;
                var qRegex = Regex.Match(userInput.Trim().ToUpper(), @"\b[ABCD]\b");
                if (qRegex.Success && (qRegex.Value == q.Answer))
                {
                    _quizState = QuizState.QUESTION;
                    output = $"Correct!\n{q.Explanation}.";
                    _botData.CorrectAnswers++;
                }
                else if (qRegex.Success && (qRegex.Value != q.Answer))
                {
                    _quizState = QuizState.QUESTION;
                    output = $"Unfortunately that isn't the correct answer.\nThe correct answer was {q.Answer}.\n{q.Explanation}";
                }
                else
                {
                    return "Whoops.. You didn't enter a valid answer. Please try again.";
                }
            }
            else
            {
                var q = (BoolQuestion)question;
                var qRegex = Regex.Match(userInput.Trim().ToLower(), @"\btrue|false\b");
                if (qRegex.Success && (bool.Parse(qRegex.Value) == q.Answer))
                {
                    _quizState = QuizState.QUESTION;
                    output = $"Correct! That statement is {q.Answer}.\n{q.Explanation}.";
                    _botData.CorrectAnswers++;
                }
                else if (qRegex.Success && (bool.Parse(qRegex.Value) != q.Answer))
                {
                    _quizState = QuizState.QUESTION;
                    output = $"Unfortunately not. That statement is {q.Answer}.\n{q.Explanation}";
                }
                else
                {
                    return "Whoops.. You didn't enter a valid answer. Please try again.";
                }
            }

            _botData.QuestionCounter++;
            if (_botData.QuestionCounter < 10)
            {
                output += "\n\nReady for your next question?";
                _quizState = QuizState.QUESTION;
            }
            else
            {
                output += "\n\nThat was the last question - well done! Are you ready to see your score?";
                _quizState = QuizState.FEEDBACK;
            }
            return output;
        }


        //==================================================//
        // Returns a string summarising the quiz round and
        // resets quiz fields and chatbot states
        private string EndQuiz()
        {
            double roundScore = _botData.GetRoundScore();

            string output = "";

            if (roundScore == 100.00)
            {
                output += $"Amazing job {_botData.UserName}! You clearly know a lot about cybersecurity!\n";
            }
            else if (roundScore > 80.00)
            {
                output += $"Impressive work {_botData.UserName}!\n";
            }
            else if (roundScore > 60.00)
            {
                output += $"Keep at it {_botData.UserName}. Your hard work will pay off soon.\n";
            }
            else if (roundScore > 50.00)
            {
                output += $"Keep going {_botData.UserName}. You'll get 'em next time!\n";
            }
            else
            {
                output += $"Hard luck {_botData.UserName}. Keep learning and you can nail the quiz next time.\n";
            }

            output += $"You answered {roundScore}% of the questions correctly.\n\nWhat's next?";

            // reset quiz fields
            _botData.QuestionCounter = 0;
            _botData.CorrectAnswers = 0;
            // reset bot states
            _quizState = QuizState.IDLE;
            _currentChatbotState = ChatBotState.IDLE;
            // update activity log
            _activityLog.Add($"Quiz completed with a score of {roundScore}%.");
            return output;
        }


        //=================================== ACTIVITY LOG METHODS ===================================//

        //=======================================================//
        // returns a formatted string containing the 10 most
        // recently performed tasks
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
            foreach (var log in _activityLog.Take(10))
            {
                response += string.Format("{0, -2}. {1}\n", pos, log);
                pos++;
            }
            // reorder acitivity log
            _activityLog.Reverse();
            _currentChatbotState = ChatBotState.IDLE;
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
            var timeRegex = Regex.Match(PreProcess(userInput), timePattern);
            if (timeRegex.Success)
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