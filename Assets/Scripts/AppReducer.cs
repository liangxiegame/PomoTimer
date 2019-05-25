
namespace PomoTimerApp
{
    public class AppReducer
    {
        public static AppState Reduce(AppState state, object action)
        {
            switch (action)
            {
                case AddTaskAction addTaskAction:
                    state.Tasks.Add(addTaskAction.Task);
                    return state;

                case RemoveTaskAction removeTaskAction:
                    state.Tasks.Remove(removeTaskAction.Task);
                    return state;

                case UpdateTaskAction updateTaskAction:
                    
                    return state;
            }

            return state;
        }
    }
}