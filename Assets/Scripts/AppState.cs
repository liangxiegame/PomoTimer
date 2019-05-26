using System.Collections.Generic;
using QFramework.UIWidgets.ReduxPersist;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;

namespace PomoTimerApp
{
    public enum PageMode
    {
        List,
        Finished,
        Setting
    }

    public static class PageModeToTitle
    {
        public static string ToTitle(this PageMode mode)
        {
            if (mode == PageMode.List)
            {
                return "任务列表";
            }
            else if (mode == PageMode.Finished)
            {
                return "完成列表";
            }
            else
            {
                return "设置";
            }
        }
    }

    public enum ThemeColor
    {
        Red,
        Purple
    }

    public static class ThemeColor2MaterialColor
    {
        public static MaterialColor ToMaterialColor(this ThemeColor color)
        {
            if (color == ThemeColor.Red)
            {
                return Colors.red;
            }

            if (color == ThemeColor.Purple)
            {
                return Colors.purple;
            }

            return Colors.grey;

        }
    }
    
    public class AppState : AbstractPersistState<AppState>
    {
        public List<Task> Tasks = new List<Task>();

        public PageMode PageMode = PageMode.List;
        
        public ThemeColor ThemeColorType = ThemeColor.Red;

        public int PomoMinutes = 25;
    }
}