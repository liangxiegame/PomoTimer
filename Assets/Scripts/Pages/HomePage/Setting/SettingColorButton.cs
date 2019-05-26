using System;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class SettingColorButton : StatelessWidget
    {
        public string title   { get; }
        public Color  color   { get; }
        public Action onClick { get; }

        public SettingColorButton(string title, Color color, Action onClick)
        {
            this.title = title;
            this.color = color;
            this.onClick = onClick;
        }

        public override Widget build(BuildContext context)
        {
            return new Padding(
                padding: EdgeInsets.only(left: 8),
                child:
                new FlatButton(
                    color: color,
                    child: new Text(title,
                        style: new TextStyle(
                            fontSize: 16,
                            color: Colors.white,
                            fontWeight: FontWeight.bold
                        )
                    ),
                    onPressed: () => { onClick(); }));
        }
    }
}