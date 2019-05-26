using System;
using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class TaskWidget : StatelessWidget
    {
        private Task mTaskData;

        private Action mOnRemove;
        private Action mOnComplete;

        public TaskWidget(Task taskData,Action onComplete, Action onRemove)
        {
            mTaskData = taskData;
            mOnComplete = onComplete;
            mOnRemove = onRemove;
        }


        public override Widget build(BuildContext context)
        {
            return new Container(
                color: Colors.white,
                child: new Card(
                    margin:EdgeInsets.all(4),
                    child: new ListTile(
                        leading: new IconButton(
                            icon:new Icon(
                                Icons.check_circle_outline,
                                color:mTaskData.Done ? Colors.green : Theme.of(context).primaryColor)
                            ,
                            onPressed:()=>mOnComplete()
                        ),
                        title: new Container(
                            margin:EdgeInsets.symmetric(4,16),
                            height: 72,
                            child: new Row(
                                mainAxisAlignment:MainAxisAlignment.start,
                                children:new List<Widget>()
                                {
                                    new Padding(
                                        padding:EdgeInsets.only(left:8),
                                        child: new Column(
                                            crossAxisAlignment:CrossAxisAlignment.start,
                                            mainAxisAlignment:MainAxisAlignment.center,
                                            children:new List<Widget>()
                                            {
                                                new Text(mTaskData.Title
                                                ,style:new TextStyle(
                                                    fontSize:18,
                                                    fontWeight:FontWeight.bold
                                                    )
                                                ),
                                                new Text(
                                                    mTaskData.Description,
                                                    style:new TextStyle(
                                                        fontSize:16,
                                                        fontWeight:FontWeight.normal
                                                        )),
                                                new Text($"{mTaskData.PomoCount} pomodoro",
                                                    style:new TextStyle(
                                                        fontSize:16,
                                                        fontWeight:FontWeight.normal
                                                    )
                                                )
                                            }
                                            )
                                        )
                                }
                                ) 
                            
                        ),
                        trailing: new IconButton(
                            icon: new Icon(
                                icon: Icons.delete,
                                color: Theme.of(context).primaryColor
                            ),
                            onPressed: () => { mOnRemove(); }
                        )
                    )
                )
            );
        }
    }
}