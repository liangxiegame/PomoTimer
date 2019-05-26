using System;
using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class FinishedList : StatelessWidget
    {
        public Action<string> OnMsg { get; }

        public FinishedList(Action<string> onMsg)
        {
            this.OnMsg = onMsg;
        }
        
        public override Widget build(BuildContext context)
        {
            return new Container(

                child: new Center(
                    child: new StoreConnector<AppState, List<Task>>(
                        converter: state => state.Tasks.Where(task => task.Done).ToList(),
                        builder: (buildContext, model, dispatcher) =>
                        {
                            if (model.Count > 0)
                            {
                                return ListView.builder(
                                    itemCount: model.Count,
                                    itemBuilder: (context1, index) =>
                                    {
                                        var taskData = model[index];

                                        return new TaskWidget(
                                            taskData,
                                            onComplete: () =>
                                            {
                                                taskData.Done = true;
                                                dispatcher.dispatch(new UpdateTaskAction(taskData));
                                            },
                                            onRemove: () =>
                                            {
                                                dispatcher.dispatch(new RemoveTaskAction(taskData));

                                                OnMsg($"{taskData.Title} Removed");

                                            });
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