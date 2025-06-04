using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10445832_PROG6221_POE_GUI.Classes
{
    public class TaskReminder
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Reminder { get; set; }
        public DateTime Created { get; set; }

        public TaskReminder() { }

        public TaskReminder(string title, string description, DateTime reminder)
        {
            Title = title;
            Description = description;
            Reminder = reminder;
            Created = DateTime.Now;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Title: {Title}");
            sb.AppendLine($"Description: {Description}");
            sb.AppendLine($"Reminder Date: {Reminder.ToString()}");
            sb.AppendLine($"Created At: {Created.ToString()}");
            return sb.ToString();
        }

    }
}
////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\