// References
// https://gemini.google.com

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ST10445832_PROG6221_PoE.Classes
{

    [XmlRoot("Tasks", Namespace="")]
    public class TasksStore
    {
        [XmlElement("Task")]
        public List<TaskReminder> Tasks { get; set; }
        public TasksStore()
        {
            Tasks = new List<TaskReminder>();
        }
    }

    public class TaskReminder
    {
        [XmlElement("TaskNumber")]
        public int TaskNumber { get; set; }
        [XmlElement("Title")]
        public string Title { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlElement("Reminder")]
        public DateTime Reminder { get; set; }
        [XmlElement("Created")]
        public DateTime Created { get; set; }
        public string OriginalInput { get; set; }

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