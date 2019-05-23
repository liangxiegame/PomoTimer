using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class RoundedButton : StatefulWidget
    {
        public RoundedButton(string text)
        {
            Text = text;
        }

        public string Text { get; }

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
                width: 140,
                height: 140,
                decoration: new BoxDecoration(
                    color: Color.fromRGBO(220, 220, 220, 220),
                    borderRadius: BorderRadius.circular(100f),
                    boxShadow: new List<BoxShadow>()
                    {
                        new BoxShadow(
                            color: Color.fromRGBO(220, 220, 220, 220),
                            blurRadius: 0f
                        )
                    }
                ),
                child:new Center(
                    child:new Text(
                        widget.Text.ToUpper(),
                        textAlign:TextAlign.center,
                        style:new TextStyle(
                            color:Colors.black,
                            fontSize:24
                            )
                        )
                    )

            );
        }
    }
}