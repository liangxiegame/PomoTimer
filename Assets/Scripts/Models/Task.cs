using System;

namespace PomodoroApp
{
    public class Task
    {
        public string Id;

        public string Title;

        public string Description;

        public bool Done;

        public int PomCount = 0;

        public Task(string id, string title, string description, bool done = false, int pomCount = 0)
        {
            Id = id ?? Guid.NewGuid().ToString();
            Title = title;
            Description = description;
            Done = done;
            PomCount = pomCount;
        }
    }
}