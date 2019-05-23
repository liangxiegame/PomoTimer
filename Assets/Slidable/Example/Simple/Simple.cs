


using System.Collections.Generic;
using QF.Slidable;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;


namespace SlidableExample
{
  public class Simple : UIWidgetsPanel
  {
    protected override void OnEnable()
    {
      base.OnEnable();
      FontManager.instance.addFont(Resources.Load<Font>(path: "MaterialIcons-Regular"), "Material Icons");
    }

    protected override Widget createWidget()
    {
      return new MaterialApp(
        title: "Flutter Slidable Demo",
        theme: new ThemeData(
          primarySwatch: Colors.blue
        ),
        home: new MyHomePage()
      );
    }
  }
  
  class MyHomePage : StatelessWidget
  {
    public override Widget build(BuildContext context)
    {
      return new Scaffold(
        appBar: new AppBar(
          title: new Text("Flutter Slidable Demo")
        ),
        body: ListView.builder(
          itemCount: 100,
          itemBuilder: (context2, index) => new Slidable(
            actionPane: new SlidableDrawerActionPane(),
            actions: new List<Widget>
            {
              new IconSlideAction(
                caption: "Archive",
                color: Colors.blue,
                icon: Icons.archive
              ),
              new IconSlideAction(
                caption: "Share",
                color: Colors.indigo,
                icon: Icons.share
              )
            },
            secondaryActions: new List<Widget>
            {
              new IconSlideAction(
                caption: "More",
                color: Colors.grey.shade200,
                icon: Icons.more_horiz),
              new IconSlideAction(
                caption: "Delete",
                color: Colors.red,
                icon: Icons.delete
              )
            },
            dismissal: new SlidableDismissal(child: new SlidableDrawerDismissal()),
            child: new ListTile(title: new Text($"{index}"))
          ))
      );
    }
  }
}