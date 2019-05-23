using System;
using System.Collections.Generic;
using QF.Slidable;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomodoroApp
{
    public class TaskWidget : StatefulWidget
    {
        public Task   Task;
        public Action OnRemoved;
        public Action OnUpdated;

        public TaskWidget(Task task, Action onRemoved, Action onUpdated)
        {
            Task = task;
            OnRemoved = onRemoved;
            OnUpdated = onUpdated;
        }

        public override State createState()
        {
            return new TaskWidghetState();
        }
    }

    class TaskWidghetState : State<TaskWidget>
    {
        public override Widget build(BuildContext context)
        {
            var task = widget.Task;

            // Slidable 的其他内容
            return new Slidable(
                actionExtentRatio: 0.25f,
                child: new Container(
                    color: Colors.white,
                    child: new Card(
                        margin: EdgeInsets.fromLTRB(4, 4, 4, 4),
                        child: new Container(
                            height: 72.0f,
                            margin: EdgeInsets.fromLTRB(16, 4, 16, 4),
                            child: new Row(
                                mainAxisAlignment: MainAxisAlignment.start,
                                children: new List<Widget>()
                                {
                                    new Icon(
                                        task.Done
                                            ? Icons.check_circle
                                            : Icons.check_circle_outline,
                                        color: task.Done ? Colors.green : Colors.red
                                    ),
                                    new Padding(
                                        padding: EdgeInsets.only(left: 8),
                                        child: new Column(
                                            crossAxisAlignment: CrossAxisAlignment.start,
                                            mainAxisAlignment: MainAxisAlignment.center,
                                            children: new List<Widget>()
                                            {
                                                new Hero(
                                                    transitionOnUserGestures: true,
                                                    tag: "text-" + task.Id,
                                                    child: new Text(
                                                        task.Title,
                                                        style: new TextStyle(
                                                            fontSize: 18,
                                                            fontWeight: FontWeight.bold
                                                        )
                                                    )
                                                ),
                                                new Text(
                                                    task.Description,
                                                    style: new TextStyle(
                                                        fontSize: 16f,
                                                        fontWeight: FontWeight.normal
                                                    )
                                                ),
                                                new Text(
                                                    task.PomCount + " pomodoro",
                                                    style: new TextStyle(
                                                        fontSize: 16,
                                                        fontWeight: FontWeight.normal
                                                    )
                                                )
                                            }
                                        )
                                    )
                                }
                            ))
                    )
                ),
                actions: new List<Widget>()
                {
                    new IconButton(
                        icon: new Icon(
                            Icons.archive,
                            size: 32.0f,
                            color: Colors.blue
                        )
                        ,
                        onPressed: () =>
                        {
                            task.Done = true;
                            widget.OnUpdated();
                        }
                    )
                },
                secondaryActions: new List<Widget>()
                {
                    new IconButton(
                        icon: new Icon(
                            Icons.delete,
                            size: 31.0f,
                            color: Colors.red
                        ),
                        onPressed: () =>
                        {
//                        await Manager().removeTask(task);
                            widget.OnRemoved();
                        }
                    )
                });
        }
    }
}