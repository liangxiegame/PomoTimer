using System;
using System.Collections.Generic;
using RSG;
using Unity.UIWidgets;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Material = Unity.UIWidgets.material.Material;

namespace PomodoroApp
{
    public class HomePage : StatefulWidget
    {
        public string title { get; }

        public HomePage(string title)
        {
            this.title = title;
        }

        public override State createState()
        {
            return new HomePageState();
        }
    }

    class HomePageState : State<HomePage>
    {
        Manager                          mTaskManager = new Manager();
        private GlobalKey<ScaffoldState> mScaffoldKey = GlobalKey<ScaffoldState>.key();
        private GlobalKey                mFabKey      = GlobalKey.key("fab");
        private GlobalKey                mItemKey     = GlobalKey.key("item");

        public override void initState()
        {
            base.initState();
            mTaskManager.LoadAllTasks();

            Promise.Delayed(TimeSpan.FromSeconds(1.0f))
                .Then(ShowCoachMarkFAB);
        }

        void ShowSnackBar(string msg)
        {
            var snackBar = new SnackBar(content: new Text(msg));
            mScaffoldKey.currentState.showSnackBar(snackBar);
        }

        void StartTimer(Task task)
        {
            Navigator.push(
                context,
                new MaterialPageRoute(
                    builder: buildContext => new TimerPage(
                        task: task
                    )
                )
            );
        }

        void AddTask(Dispatcher dispatcher)
        {
            Navigator.push(
                context,
                new MaterialPageRoute(
                    builder: (buildContext => new NewTaskPage())
                )
            ).Then(result =>
            {
                var task = result as Task;

                if (task != null)
                {
                    dispatcher.dispatch(new AddTaskAction(task));

                    setState(() => { ShowSnackBar("Added:" + task.Title); });
                }

            });
        }


        void ShowCoachMarkFAB()
        {

        }

        void ShowCoachMarkItem()
        {

        }



        public override Widget build(BuildContext context)
        {
            Promise.Delayed(TimeSpan.FromSeconds(1.0f))
                .Then(ShowCoachMarkItem);
            return new Scaffold(
                key: mScaffoldKey,
                floatingActionButton: new StoreConnector<AppState, AppState>(
                    converter: (state => state),
                    builder: (buildContext, model, dispatcher) =>
                    {
                        return new FloatingActionButton(
                            key: mFabKey,
                            onPressed: () => { AddTask(dispatcher); },
                            tooltip: "Add new task",
                            child: new Icon(
                                Icons.add,
                                color: Colors.white
                            ));
                    }
                ),
                body: new NestedScrollView(
                    headerSliverBuilder: (buildContext, scrolled) =>
                    {
                        return new List<Widget>()
                        {
                            new SliverAppBar(
                                expandedHeight: 80.0f,
                                floating: true,
                                pinned: false,
                                flexibleSpace: new FlexibleSpaceBar(
                                    centerTitle: true,
                                    title: new Text(widget.title,
                                        style: new TextStyle(
                                            color: Colors.white,
                                            fontSize: 20.0f,
                                            fontWeight: FontWeight.bold
                                        ))
                                )
                            )
                        };
                    },
                    body: new Container(
                        child: new StoreConnector<AppState, AppState>(
                            converter: state => state,
                            builder: (buildContext, model, dispatcher) =>
                            {
                                var tasks = model.TasksData;
                                Debug.Log("Count:" + tasks.Count);

//                            if (snapshot.connectionState != ConnectionState.done) {
//                                return Center(
//                                    child: CircularProgressIndicator(),
//                                );
//                            }

                                if (tasks != null && tasks.Count == 0)
                                {
                                    return new Center(
                                        child: new Text(
                                            "Cool! Nothing to do.",
                                            style: new TextStyle(fontSize: 24)
                                        )
                                    );
                                }
                                else
                                {
                                    return ListView.builder(
                                        padding: EdgeInsets.only(top: 0),
                                        itemCount: tasks.Count,
                                        scrollDirection: Axis.vertical,
                                        shrinkWrap: true,
                                        itemBuilder: (context1, index) =>
                                        {
                                            var item = tasks[index];
                                            return new Hero(
                                                tag: "task-" + item.Id,
                                                child: new Material(
                                                    key: index == 0 ? mItemKey : null,
                                                    child: new InkWell(
                                                        child: new TaskWidget(
                                                            task: item,
                                                            onRemoved: () =>
                                                            {

                                                                dispatcher.dispatch(new RemoveTaskAction(item));

                                                                setState(() =>
                                                                {
                                                                    ShowSnackBar("Removed:" + item.Title);
                                                                });
                                                            },
                                                            onUpdated: () =>
                                                            {
                                                                dispatcher.dispatch(new UpdateTaskAction(item));
                                                                setState(() =>
                                                                {
                                                                    ShowSnackBar("Updated" + item.Title);
                                                                });
                                                            }
                                                        ),
                                                        onTap: () => { StartTimer(item); }

                                                    )

                                                )
                                            );
                                        }
                                    );
                                }
                            })
                    )
                )
            );
        }
    }
}