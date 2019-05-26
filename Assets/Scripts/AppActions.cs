using Unity.UIWidgets.material;

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

    public class Change2ListModeAction
    {
        
    }
    
    public class Change2FinishedModeAction
    {
        
    }
    
    public class Change2SettingModeAction
    {
        
    }

    public class ChangeThemeColorAction
    {
        public ThemeColor color { get; }

        public ChangeThemeColorAction(ThemeColor color)
        {
            this.color = color;
        }
    }

    public class ChangePomoMinutesAction
    {
        public ChangePomoMinutesAction(int minutes)
        {
            Minutes = minutes;
        }

        public int Minutes { get; }
    }
    
}