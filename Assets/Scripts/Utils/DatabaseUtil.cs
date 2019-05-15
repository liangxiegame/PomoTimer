using System.Collections.Generic;

namespace PomodoroApp
{
    public class DatabaseUtil
    {
        static List<Task> DB = new List<Task>();

        public static void Insert(Task task)
        {
            DB.Add(task);
        }

        public static void Update(Task task)
        {

        }

        public static void Remove(Task task)
        {
            DB.Remove(task);
        }

        public static List<Task> GetAll()
        {
            return DB;
        }

    }
}