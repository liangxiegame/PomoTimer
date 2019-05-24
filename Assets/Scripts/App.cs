using QFramework.UIWidgets.ReduxPersist;
using Unity.UIWidgets;
using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace PomoTimerApp
{
    public class App : UIWidgetsPanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            FontManager.instance.addFont(Resources.Load<Font>("MaterialIcons-Regular"), "Material Icons");
        }

        protected override Widget createWidget()
        {
            var store = new Store<AppState>(
                reducer: AppReducer.Reduce,
                initialState: AppState.Load(),
                ReduxPersistMiddleware.create<AppState>());

            return new StoreProvider<AppState>(store: store,
                child: new MaterialApp(
                    title: "PomoTimer",
                    theme: new ThemeData(
                        primarySwatch: Colors.red
                    ),
                    home: new HomePage()
                ));
        }
    }
}