using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class Setting : StatefulWidget
    {
        public Setting(int minutes)
        {
            Minutes = minutes;
        }

        public int Minutes { get; set; }

        public override State createState()
        {
            return new SettingState();
        }
    }

    class SettingState : State<Setting>
    {
        
        public override Widget build(BuildContext context)
        {
            return new StoreConnector<AppState, int>(
                converter: state => state.PomoMinutes,
                builder: (buildContext, model, dispatcher) =>
                {
                    return new Container(
                        padding: EdgeInsets.all(16),
                        child: new Column(
                            children: new List<Widget>()
                            {
                                new Card(
                                    child: new Container(
                                        height: 80,
                                        padding: EdgeInsets.symmetric(4, 16),
                                        child:
                                        new Row(
                                            children: new List<Widget>()
                                            {
                                                new Text("主题颜色:",
                                                    style: new TextStyle(
                                                        fontWeight: FontWeight.bold,
                                                        fontSize: 16
                                                    )
                                                ),
                                                new SettingColorButton("红色", Colors.red,
                                                    () =>
                                                    {
                                                        dispatcher.dispatch(new ChangeThemeColorAction(ThemeColor.Red));
                                                    }),
                                                new SettingColorButton("紫色", Colors.purple,
                                                    () =>
                                                    {
                                                        dispatcher.dispatch(
                                                            new ChangeThemeColorAction(ThemeColor.Purple));
                                                    })
                                            })
                                    )),
                                new Card(
                                    child: new Container(
                                        height: 80,
                                        padding: EdgeInsets.symmetric(4, 16),
                                        child:
                                        new Row(
                                            children: new List<Widget>()
                                            {
                                                new Text("番茄时长:",
                                                    style: new TextStyle(
                                                        fontWeight: FontWeight.bold,
                                                        fontSize: 16
                                                    )
                                                ),
                                                new Slider(
                                                    min: 15,
                                                    max: 45,
                                                    value: widget.Minutes,
                                                    divisions: 6,
                                                    onChanged: value => setState(() => widget.Minutes = (int)value),
                                                    onChangeEnd: (value =>
                                                    {
                                                        dispatcher.dispatch(
                                                            new ChangePomoMinutesAction((int) value));
                                                    })
                                                ),
                                                new Text($"{ widget.Minutes.ToString()} min",
                                                    style: new TextStyle(
                                                        fontWeight: FontWeight.bold))
                                            })
                                    ))
                            }
                        )
                    );
                });
        }
    }
}