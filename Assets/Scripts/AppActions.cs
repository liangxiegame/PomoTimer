namespace PomoTimerApp
{
    public class AddTaskAction
    {
        public AddTaskAction(Task task)
        {
            Task = task;
        }

        public Task Task { get; }
    }

    public class RemoveTaskAction
    {
        public RemoveTaskAction(Task task)
        {
            Task = task;
        }

        public  Task Task { get; }
    }

    public class UpdateTaskAction
    {
        public UpdateTaskAction(Task task)
        {
            Task = task;
        }

        public Task Task { get; }
    }
}