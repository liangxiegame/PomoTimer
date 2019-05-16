using QFramework.UIWidgets.ReduxPersist;
using Unity.UIWidgets;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace PomodoroApp
{
    public class App : UIWidgetsPanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            FontManager.instance.addFont(Resources.Load<Font>(path: "MaterialIcons-Regular"), "Material Icons");
        }

        protected override Widget createWidget()
        {
            var store = new Store<AppState>(AppReducer.Reduce,
                AppState.Load(),
                ReduxPersistMiddleware.create<AppState>());

            return new StoreProvider<AppState>(
                store: store,
                child: new MaterialApp(
                    title: "F-Pomodoro",
                    theme: new ThemeData(
                        primarySwatch: Colors.red
                    ),
                    home: new HomePage(title: "F-Pomodoro")
                )
            );
        }
    }
}