using System.Collections.Generic;
using QFramework.UIWidgets.ReduxPersist;

namespace PomoTimerApp
{
    public class AppState : AbstractPersistState<AppState>
    {
        public List<Task> Tasks = new List<Task>();
    }
}