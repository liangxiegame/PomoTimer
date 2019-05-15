namespace PomodoroApp
{
    public class AppReducer
    {
        public static AppState Reduce(AppState prevousState, object action)
        {
            switch (action)
            {
                case AddTaskAction addTaskAction:
                    prevousState.TasksData.Add(addTaskAction.Task);
                    break;
                case RemoveTaskAction removeTaskAction:
                    prevousState.TasksData.Remove(removeTaskAction.Task);
                    break;
            }

            return prevousState;
        }
    }
}