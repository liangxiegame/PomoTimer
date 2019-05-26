
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
                case Change2FinishedModeAction _:
                    state.PageMode = PageMode.Finished;
                    return state;
                case Change2ListModeAction _:
                    state.PageMode = PageMode.List;
                    return state;
                case Change2SettingModeAction _:
                    state.PageMode = PageMode.Setting;
                    return state;
                case ChangeThemeColorAction changeThemeColorAction:
                    state.ThemeColorType = changeThemeColorAction.color;
                    return state;
                case ChangePomoMinutesAction changePomoMinutesAction:
                    state.PomoMinutes = changePomoMinutesAction.Minutes;
                    return state;
            }

            return state;
        }
    }
}