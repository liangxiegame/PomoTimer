using System;
using System.Diagnostics;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using Debug = UnityEngine.Debug;

namespace PomoTimerApp
{
    public class App : UIWidgetsPanel
    {
        
        
        protected override Widget createWidget()
        {
            return new TimerPage();
        }
    }
    
    public class TimerPage : StatefulWidget
    {
        
        public override State createState()
        {
            return new TimerPageState();
        }
    }

    class TimerPageState :State<TimerPage>
    {
        public readonly static TimeSpan DELAY = TimeSpan.FromMilliseconds(100);

        private string mTimeText = "25:00";
        
        public override void initState()
        {
            base.initState();
            
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            var timer =Window.instance.periodic(DELAY, () =>
            {
                Debug.LogFormat("DELAYED:{0}",stopwatch.Elapsed.TotalSeconds);
                
                this.setState(() =>
                    {
                        mTimeText = $"{25 - stopwatch.Elapsed.Minutes - 1}:{60 - stopwatch.Elapsed.Seconds - 1}";
                    });
            });
            
        }

        public override Widget build(BuildContext context)
        {

            
            return new Text(mTimeText, style: new TextStyle(
                color: Colors.white,
                fontSize: 50
            ));
        }
    }
}