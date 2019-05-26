using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class HomePage : StatefulWidget
    {
        public override State createState()
        {
            return new HomePageState();
        }
    }

    class HomePageState : State<HomePage>
    {
        GlobalKey<ScaffoldState> mKey = GlobalKey<ScaffoldState>.key();
        
        void ShowSnackBar(string msg)
        {
            var snackBar = new SnackBar(content: new Text(msg));
            mKey.currentState.showSnackBar(snackBar);
        }

        public override Widget build(BuildContext context)
        {
            return new Scaffold(
                key:mKey,
                floatingActionButton: new StoreConnector<AppState, AppState>(
                    converter: state => state,
                    builder: (buildContext1, model, dispatcher) =>
                    {
                        if (model.PageMode == PageMode.List)
                        {
                            return new FloatingActionButton(
                                child: new Icon(
                                    Icons.add,
                                    color: Colors.white
                                ),
                                onPressed: () =>
                                {
                                    Navigator.push(context, new MaterialPageRoute(
                                        builder: (buildContext => { return new NewTaskPage(); })
                                    )).Then(result =>
                                    {
                                        var newTask = result as Task;

                                        if (newTask != null)
                                        {
                                            dispatcher.dispatch(new AddTaskAction(newTask));
                                        }
                                    });
                                }
                            );
                        }
                        else
                        {
                            return null;
                        }
                    }
                ),
                drawer: new MenuDrawer(),
                body: new NestedScrollView(
                    headerSliverBuilder: (buildContext, scrolled) =>
                    {
                        return new List<Widget>
                        {
                            new SliverAppBar(
                                expandedHeight: 80.0f,
                                floating: true,
                                pinned: false,
                                flexibleSpace: new FlexibleSpaceBar(
                                    centerTitle: true,
                                    title: new StoreConnector<AppState, PageMode>(
                                        converter: state => state.PageMode,
                                        builder: (buildContext1, model, dispatcher) => new Text(
                                            model.ToTitle(),
                                            style: new TextStyle(
                                                fontSize: 20.0f,
                                                fontWeight: FontWeight.bold,
                                                color: Colors.white
                                            )))
                                )
                            )
                        };
                    },
                    body: new StoreConnector<AppState, AppState>(
                        converter: state => state,
                        builder: (buildContext, model, dispatcher) =>
                        {
                            if (model.PageMode == PageMode.List)
                            {
                                return new TaskList(ShowSnackBar);
                            }
                            else if (model.PageMode == PageMode.Finished)
                            {
                                return new FinishedList(ShowSnackBar);
                            }
                            else
                            {
                                return new Setting(model.PomoMinutes);
                            }
                        }
                    )
                )
            );
        }
    }
}