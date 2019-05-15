using System;
using System.Collections.Generic;
using System.Diagnostics;
using PomodoroApp.UI;
using RSG;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.async;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using Debug = UnityEngine.Debug;

namespace PomodoroApp
{
    public class TimerPage : StatefulWidget
    {
        public TimerPage(Task task)
        {
            Task = task;
        }

        public Task Task { get; }

        public override State createState()
        {
            return new TimerPageState();
        }
    }

    class TimerPageState : SingleTickerProviderStateMixin<TimerPage>
    {
        private Timer mTimer;

        private Task Task => widget.Task;

        private string mTimeText   = "";
        private string mButtonText = "";

        private int mMinutes = 25;

        Stopwatch                        mStopwatch = new Stopwatch();
        private static readonly TimeSpan DELAY      = TimeSpan.FromMilliseconds(100f);

        private float               mBegin = 0.0f;
        private Animation<float>    mHeightSize;
        private AnimationController mController;


        private float mBrightness = 0.5f;
        private bool  mIsKeptOn   = false;

        void UpdateClock()
        {
            var currentMinute = (int) mStopwatch.Elapsed.TotalMinutes;

            if (currentMinute == mMinutes)
            {
                if (Navigator.canPop(context))
                {
                    Task.PomCount += 1;

                    Navigator.of(context).pop(Task);
                }

                return;
            }
            
            setState(() =>
            {
                mTimeText =
                    $"{(mMinutes - currentMinute - 1).ToString().PadLeft(2, '0')}:{(60 - (int) mStopwatch.Elapsed.TotalSeconds % 60 - 1).ToString().PadLeft(2, '0')}";
            });

            if (mStopwatch.IsRunning)
            {
                setState(() => { mButtonText = "Running"; });
            }
            else if ((int) mStopwatch.Elapsed.TotalSeconds == 0)
            {
                setState(() =>
                {
                    mTimeText = $"{mMinutes}:00";
                    mButtonText = "Start";
                });
            }
            else
            {
                setState(() => { mButtonText = "Paused"; });
            }
        }


        public override void initState()
        {
            base.initState();

            mController = new AnimationController(
                duration: TimeSpan.FromMinutes(mMinutes),
                vsync: this
            );

            mController.addStatusListener(state => { Debug.Log("-----animation state:" + state); });

            KeepScreenAwake();

            mTimer =  Window.instance.periodic(DELAY, UpdateClock);
        }

        void KeepScreenAwake()
        {
        }

        public override void dispose()
        {
            mController.dispose();
            mStopwatch.Stop();
            mTimer.cancel();
            base.dispose();
        }

        public override Widget build(BuildContext context)
        {
            mHeightSize = new FloatTween(begin: mBegin, end: MediaQuery.of(context).size.height - 65)
                .animate(new CurvedAnimation(
                    parent: mController,
                    curve: Curves.easeInOut
                ));

            var size = new Size(MediaQuery.of(context).size.width, mHeightSize.value * 0.9f);

            return new Scaffold(
                backgroundColor: Colors.white,
                body: new Material(
                    child: new Stack(
                        children: new List<Widget>()
                        {
                            new AnimatedBuilder(
                                animation: mController,
                                builder: (buildContext, child) =>
                                {
                                    return new DemoBody(
                                        size: size,
                                        color: Theme.of(context).primaryColor
                                    );
                                }

                            ),
                            new Padding(
                                padding: EdgeInsets.only(top: 32.0f, left: 4.0f, right: 4.0f),
                                child: new Row(
                                    children: new List<Widget>()
                                    {
                                        new IconButton(
                                            icon: new Icon(
                                                Icons.arrow_back,
                                                size: 40.0f,
                                                color: Colors.grey
                                            ),
                                            onPressed: () =>
                                            {
                                                if ((int) mStopwatch.Elapsed.TotalMinutes > 0)
                                                {

                                                    Task.PomCount++;
                                                    Navigator.of(context).pop(Task);
                                                }
                                                else
                                                {
                                                    Navigator.of(context).pop();
                                                }
                                            }),
                                        new Spacer(),
                                        new IconButton(
                                            icon: new Icon(
                                                Icons.done_all,
                                                size: 32.0f,
                                                color: Theme.of(context).primaryColor
                                            ),
                                            onPressed: () =>
                                            {
                                                var task = Task;
                                                task.Done = true;
                                                task.PomCount++;
                                                Navigator.of(context).pop(task);
                                            }
                                        )
                                    }
                                )
                            ),
                            new Align(
                                alignment: Alignment.topCenter,
                                child: new Container(
                                    margin: EdgeInsets.only(top: 100),
                                    child: new Column(
                                        mainAxisSize: MainAxisSize.min,
                                        children: new List<Widget>()
                                        {
                                            new Hero(
                                                transitionOnUserGestures: true,
                                                tag: $"text-{widget.Task.Id}",
                                                child: new Text(
                                                    widget.Task.Title,
                                                    style: new TextStyle(fontSize: 30.0f, color: Colors.grey)
                                                )
                                            ),
                                            new Text(
                                                widget.Task.Description,
                                                style: new TextStyle(color: Colors.grey)
                                            )
                                        }
                                    )
                                )
                            ),
                            new Align(
                                alignment: Alignment.center,
                                child: new Container(
                                    margin: EdgeInsets.only(bottom: 100),
                                    child: new Center(
                                        child: new Column(
                                            mainAxisSize: MainAxisSize.min,
                                            children: new List<Widget>
                                            {
                                                new Text(
                                                    mTimeText,
                                                    style: new TextStyle(
                                                        fontSize: 54.0f,
                                                        color: Colors.black,
                                                        fontWeight: FontWeight.bold)
                                                )
                                            }
                                        )
                                    )
                                )
                            ),
                            new Align(
                                alignment: Alignment.bottomCenter,
                                child: new Container(
                                    margin: EdgeInsets.only(bottom: 32),
                                    child: new GestureDetector(
                                        child: new RoundedButton(text: mButtonText),
                                        onTap: () =>
                                        {
                                            if (mStopwatch.IsRunning)
                                            {
                                                mStopwatch.Stop();
                                                mController.stop(canceled: false);
                                            }
                                            else
                                            {
                                                mBegin = 50.0f;
                                                mStopwatch.Start();
                                                mController.forward();
                                            }

                                            UpdateClock();
                                        })
                                )
                            )
                        }

                    )
                )
            );
        }
    }

    class RoundedButton : StatefulWidget
    {
        public string Text;

        public RoundedButton(string text)
        {
            Text = text;
        }


        public override State createState()
        {
            return new RoundedButtonState();
        }
    }

    class RoundedButtonState : State<RoundedButton>
    {
        public override Widget build(BuildContext context)
        {
            return new Container(
                width: 140.0f,
                height: 140.0f,
                decoration: new BoxDecoration(
                    color: Color.fromRGBO(220, 220, 220, 220),
                    borderRadius: BorderRadius.circular(100.0f),
                    boxShadow: new List<BoxShadow>()
                    {
                        new BoxShadow(
                            color: Color.fromRGBO(220, 220, 220, 220),
                            blurRadius: 0.0f
                        )
                    }
                ),
                child: new Center(
                    child: new Text(
                        widget.Text.ToUpper(),
                        style: new TextStyle(
                            fontSize: 24.0f,
                            color: Colors.black
                        )
                    )
                )
            );
        }
    }
}