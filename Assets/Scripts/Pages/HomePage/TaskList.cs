using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class TaskList : StatelessWidget
    {
        public override Widget build(BuildContext context)
        {
            return new Container(

                child: new Center(
                    child: new StoreConnector<AppState, List<Task>>(
                        converter: state => state.Tasks.Where(task => !task.Done).ToList(),
                        builder: (buildContext, model, dispatcher) =>
                        {
                            if (model.Count > 0)
                            {
                                return ListView.builder(
                                    itemCount: model.Count,
                                    itemBuilder: (context1, index) =>
                                    {
                                        var taskData = model[index];

                                        return new InkWell(

                                            child: new TaskWidget(
                                                taskData,
                                                onComplete: () =>
                                                {
                                                    taskData.Done = true;
                                                    dispatcher.dispatch(new UpdateTaskAction(taskData));
                                                },
                                                onRemove: () =>
                                                {
                                                    dispatcher.dispatch(new RemoveTaskAction(taskData));
                                                }),
                                            onTap: () =>
                                            {
                                                Navigator.of(context).push(new MaterialPageRoute(
                                                    builder: buildContext1 => new TimerPage(taskData)
                                                )).Then(result =>
                                                {
                                                    var task = result as Task;
                                                    if (task != null)
                                                    {
                                                        dispatcher.dispatch(new UpdateTaskAction(task));
                                                    }

                                                });
                                            }
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
                );
        }
    }
}