using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class MenuDrawer : StatelessWidget
    {
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, PageMode>(
                converter: state => state.PageMode,
                builder: (buildContext, model, dispatcher) => new Container(
                    width: 120,
                    child: new Drawer(
                        child: new Column(
                            crossAxisAlignment: CrossAxisAlignment.center,
                            children: new List<Widget>()
                            {
                                new Container(
                                    height: 80,
                                    color: Colors.red
                                ),
                                new GestureDetector(
                                    child: new Container(
                                        color: model == PageMode.List ? Colors.grey[300] : Colors.white,
                                        padding: EdgeInsets.symmetric(20, 4),
                                        child: new Row(
                                            mainAxisAlignment: MainAxisAlignment.center,
                                            children: new List<Widget>()
                                            {
                                                new Icon(
                                                    Icons.list,
                                                    color: Colors.red
                                                ),
                                                new Padding(
                                                    padding: EdgeInsets.only(left: 10),
                                                    child: new Text("未完成")
                                                )
                                            })
                                    ),
                                    onTap: () =>
                                    {
                                        if (model == PageMode.List)
                                        {

                                        }
                                        else
                                        {
                                            dispatcher.dispatch(new Change2ListModeAction());
                                        }

                                    }
                                ),
                                new GestureDetector(
                                    child: new Container(
                                        color: model == PageMode.Finished ? Colors.grey[300] : Colors.white,
                                        padding: EdgeInsets.symmetric(20, 4),
                                        child: new Row(
                                            mainAxisAlignment: MainAxisAlignment.center,
                                            children: new List<Widget>()
                                            {
                                                new Icon(
                                                    Icons.done,
                                                    color: Colors.red
                                                ),
                                                new Padding(
                                                    padding: EdgeInsets.only(left: 10),
                                                    child: new Text("已完成")
                                                )
                                            }
                                        )
                                    ),
                                    onTap: () =>
                                    {
                                        if (model == PageMode.Finished)
                                        {

                                        }
                                        else
                                        {
                                            dispatcher.dispatch(new Change2FinishedModeAction());
                                        }

                                    })
                            }
                        )
                    )));
        }
    }
}