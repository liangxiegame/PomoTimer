using System.Collections.Generic;
using QFramework.UIWidgets.ReduxPersist;

namespace PomodoroApp
{
    public class AppState : AbstractPersistState<AppState>
    {
        public List<Task> TasksData = new List<Task>();
    }
}