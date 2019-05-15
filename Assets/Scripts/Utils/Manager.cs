using System.Collections.Generic;

namespace PomodoroApp
{
    public class Manager
    {
        public List<Task> TasksData;
        
        public void AddNewTask(Task task)
        {
            DatabaseUtil.Insert(task);
        }

        public void UpdateTask(Task task)
        {
            DatabaseUtil.Update(task);
        }

        public void RemoveTask(Task task)
        {
            DatabaseUtil.Remove(task);
        }

        public void LoadAllTasks()
        {
            TasksData = DatabaseUtil.GetAll();
        }
    }
}