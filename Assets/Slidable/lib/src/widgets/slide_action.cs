



using System;
using System.Collections.Generic;
using QF.Slidable;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.service;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;


namespace QF.Slidable
{
  /// Abstract class for slide actions that can close after [onTap] occurred.
  abstract class ClosableSlideAction : StatelessWidget
  {

    public const bool _kCloseOnTap = true;

    /// Creates a slide that closes when a tap has occurred if [closeOnTap]
    /// is [true].
    ///
    /// The [closeOnTap] argument must not be null.
    public ClosableSlideAction(
      Key key,
      VoidCallback onTap,
      bool closeOnTap = _kCloseOnTap
    ) :
      base(key: key)
    {
      D.assert(closeOnTap != null);
      this.onTap = onTap;
      this.closeOnTap = closeOnTap;
    }

    /// A tap has occurred.
    VoidCallback onTap;

    /// Whether close this after tap occurred.
    ///
    /// Defaults to true.
    bool closeOnTap;

    /// Calls [onTap] if not null and closes the closest [Slidable]
    /// that encloses the given context.
    void _handleCloseAfterTap(BuildContext context)
    {
      onTap?.Invoke();
      Slidable.of(context)?.close();
    }

    public override Widget build(BuildContext context)
    {
      return new GestureDetector(
        onTap: !closeOnTap
          ? new GestureTapCallback(() => { onTap(); })
          : new GestureTapCallback(() => _handleCloseAfterTap(context)),
        child: buildAction(context)
      );
    }

    public abstract Widget buildAction(BuildContext context);
  }

  /// A basic slide action with a background color and a child that will
  /// be center inside its area.
  class SlideAction : ClosableSlideAction
  {
    /// Creates a slide action with a child.
    ///
    /// The `color` argument is a shorthand for `decoration:
    /// BoxDecoration(color: color)`, which means you cannot supply both a `color`
    /// and a `decoration` argument. If you want to have both a `color` and a
    /// `decoration`, you can pass the color as the `color` argument to the
    /// `BoxDecoration`.
    ///
    /// The [closeOnTap] argument must not be null.
    public SlideAction(
      Key key,
      Widget child,
      VoidCallback onTap,
      Color color,
      Decoration decoration,
      bool closeOnTap = _kCloseOnTap
    ) : base(key: key, onTap: onTap, closeOnTap: closeOnTap)
    {
      D.assert(child != null);
      D.assert(decoration == null || decoration.debugAssertIsValid());
      D.assert(
        color == null || decoration == null,
        () =>
          "Cannot provide both a color and a decoration\nThe color argument is just a shorthand for \"decoration:  new BoxDecoration(color: color) + \".");
      decoration =
        decoration ?? (color != null ? new BoxDecoration(color: color) : null);
    }

    /// The decoration to paint behind the [child].
    ///
    /// A shorthand for specifying just a solid color is available in the
    /// constructor: set the `color` argument instead of the `decoration`
    /// argument.
    Decoration decoration;

    /// The [child] contained by the slide action.
    Widget child;

    public override Widget buildAction(BuildContext context)
    {
      return new Container(
        decoration: decoration,
        child: new Center(
          child: child
        )
      );
    }
  }

  /// A basic slide action with an icon, a caption and a background color.
  class IconSlideAction : ClosableSlideAction
  {
    /// Creates a slide action with an icon, a [caption] if set and a
    /// background color.
    ///
    /// The [closeOnTap] argument must not be null.
    public IconSlideAction(
      Key key = null,
      IconData icon = null,
      Widget iconWidget = null,
      string caption = null
      ,
      Color color = null,
      Color foregroundColor = null,
      VoidCallback onTap = null,
      bool closeOnTap = _kCloseOnTap
    ) : base(key: key, onTap: onTap, closeOnTap: closeOnTap)
    {
      this.color = color ?? Colors.white;
      D.assert(icon != null || iconWidget != null,
        () => "Either set icon or iconWidget.");
      this.icon = icon;
      this.iconWidget = iconWidget;
      this.caption = caption;
      this.color = color;
      this.foregroundColor = foregroundColor;
    }

    /// The icon to show.
    IconData icon;

    /// A custom widget to represent the icon.
    /// If both [icon] and [iconWidget] are set, they will be shown at the same
    /// time.
    Widget iconWidget;

    /// The caption below the icon.
    String caption;

    /// The background color.
    ///
    /// Defaults to true.
    Color color;

    /// The color used for [icon] and [caption].
    Color foregroundColor;

    public override Widget buildAction(BuildContext context)
    {
      Color estimatedColor =
        ThemeData.estimateBrightnessForColor(color) == Brightness.light
          ? Colors.black
          : Colors.white;

      List<Widget> widgets = new List<Widget>();

      if (icon != null)
      {
        widgets.Add(
          new Flexible(
            child: new Icon(
              icon,
              color: foregroundColor ?? estimatedColor
            )
          )
        );
      }

      if (iconWidget != null)
      {
        widgets.Add(
          new Flexible(child: iconWidget)
        );
      }

      if (caption != null)
      {
        widgets.Add(
          new Flexible(
            child: new Text(
              caption,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context)
                .primaryTextTheme
                .caption
                .copyWith(color: foregroundColor ?? estimatedColor)
            )
          )
        );
      }

      return new Container(
        color: color,
        child: new Center(
          child: new Column(
            mainAxisSize: MainAxisSize.min,
            children: widgets
          )
        )
      );
    }
  }
}