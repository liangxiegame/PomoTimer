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
        public override Widget build(BuildContext context)
        {
            return new Scaffold(
                floatingActionButton: new StoreConnector<AppState,AppState>(
                    converter:state => state,
                    builder:(buildContext1, model, dispatcher) =>
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
                                    title: new Text("PomoTimer",
                                        style: new TextStyle(
                                            fontSize: 20.0f,
                                            fontWeight: FontWeight.bold,
                                            color: Colors.white
                                        ))
                                )
                            )
                        };
                    },
                    body: new Container(

                        child: new Center(
                            child:new StoreConnector<AppState,List<Task>>(
                                converter:state => state.Tasks,
                                builder:(buildContext, model, dispatcher) =>
                                {
                                    if (model.Count > 0)
                                    {
                                        return ListView.builder(
                                            itemCount:model.Count,
                                            itemBuilder: (context1, index) =>
                                            {
                                                var taskData = model[index];

                                                return new ListTile(
                                                    title: new Text(taskData.Title),
                                                    
                                                    trailing: new IconButton(
                                                        icon: new Icon(
                                                            icon: Icons.delete,
                                                            color:Theme.of(context).primaryColor,
                                                            size:31
                                                        ),
                                                        onPressed: () =>
                                                        {
                                                            dispatcher.dispatch(new RemoveTaskAction(taskData));
                                                        }
                                                    )
                                                );
                                            }
                                        );
                                    }
                                    else
                                    {
                                        return new Text("Cool!Nothing to do",
                                            style: new TextStyle(
                                                fontSize: 24
                                            )
                                        );
                                    }
                                }
                                )
                        )

                    )
                )
            );
        }
    }
}