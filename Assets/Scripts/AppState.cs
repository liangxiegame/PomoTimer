using System.Collections.Generic;

namespace PomodoroApp
{
    public class AppState
    {
        public List<Task> TasksData = new List<Task>()
        {
            new Task(null,"录制课时","要录制 10 个课时")
        };
    }
}