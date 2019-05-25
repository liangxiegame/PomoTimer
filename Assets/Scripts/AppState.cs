using System.Collections.Generic;
using QFramework.UIWidgets.ReduxPersist;

namespace PomoTimerApp
{
    public enum PageMode
    {
        List,
        Finished
    }
    
    
    public class AppState : AbstractPersistState<AppState>
    {
        public List<Task> Tasks = new List<Task>();

        public PageMode PageMode = PageMode.List;

    }
}