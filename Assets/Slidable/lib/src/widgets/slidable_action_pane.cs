

using System.Collections.Generic;
using System.Linq;
using QF.Slidable;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

class _SlidableStackActionPane : StatelessWidget
{
  public _SlidableStackActionPane(
    SlidableData data,
    Widget child,
    Key key = null
  ) : base(key: key)
  {
    _animation = new OffsetTween(
      begin: Offset.zero,
      end: data.createOffset(data.totalActionsExtent * data.actionSign)
    ).animate(data.actionsMoveAnimation);

  }

  Widget            child;
  SlidableData      data;
  Animation<Offset> _animation;


  public override Widget build(BuildContext context)
  {
    if (data.actionsMoveAnimation.isDismissed)
    {
      return data.slidable.child;
    }

    return new Stack(
      children: new List<Widget>
      {
        child,
        new SlideTransition(
          position: _animation,
          child: data.slidable.child
        )
      }
    );
  }
}

/// An action pane that creates actions which stretch while the item is sliding.
class SlidableStrechActionPane : StatelessWidget
{
  public SlidableStrechActionPane(Key key) : base(key: key)
  {

  }

  public override Widget build(BuildContext context)
  {
    SlidableData data = SlidableData.of(context);

    var animation = new FloatTween(
      begin: 0.0f,
      end: data.totalActionsExtent
    ).animate(data.actionsMoveAnimation);

    return new _SlidableStackActionPane(
      data: data,
      child: Positioned.fill(
        child: new AnimatedBuilder(
          animation: data.actionsMoveAnimation,
          builder: (context1, child1) =>
          {
            return new FractionallySizedBox(
              alignment: data.alignment,
              widthFactor: data.directionIsXAxis ? animation.value : null as float?,
              heightFactor: data.directionIsXAxis ? null as float? : animation.value,
              child: new Flex(
                direction: data.direction,
                children: data
                  .buildActions(context)
                  .Select(a=>new Expanded(child:a) as Widget)
                  .ToList()
              )
            );
          }
        )
      )
    );
  }
}

/// An action pane that creates actions which stay behind the item while it's sliding.
class SlidableBehindActionPane : StatelessWidget {
  public SlidableBehindActionPane(Key key) : base(key: key) {}

  public override Widget build(BuildContext context) {
    SlidableData data = SlidableData.of(context);

    return new _SlidableStackActionPane(
      data: data,
      child: Positioned.fill(
        child: new FractionallySizedBox(
          alignment: data.alignment,
          widthFactor: data.actionPaneWidthFactor,
          heightFactor: data.actionPaneHeightFactor,
          child: new Flex(
            direction: data.direction,
            children: data
                .buildActions(context)
                .Select(a=>new Expanded(child:a) as Widget)
                .ToList()
          )
        )
      )
    );
  }
}

/// An action pane that creates actions which follow the item while it's sliding.
class SlidableScrollActionPane : StatelessWidget
{

  public SlidableScrollActionPane(Key key) : base(key: key)
  {

  }

  public override Widget build(BuildContext context)
  {
    SlidableData data = SlidableData.of(context);

    var alignment = data.alignment;
    var animation = new OffsetTween(
      begin: new Offset(alignment.x, alignment.y),
      end: Offset.zero
    ).animate(data.actionsMoveAnimation);

    return new _SlidableStackActionPane(
      data: data,
      child: Positioned.fill(
        child: new FractionallySizedBox(
          alignment: data.alignment,
          widthFactor: data.actionPaneWidthFactor,
          heightFactor: data.actionPaneHeightFactor,
          child: new SlideTransition(
            position: animation,
            child: new Flex(
              direction: data.direction,
              children: data
                .buildActions(context)
                .Select(a => new Expanded(child: a) as Widget)
                .ToList()
            )
          )
        )
      )
    );
  }
}

/// An action pane that creates actions which animate like drawers while the item is sliding.
class SlidableDrawerActionPane : StatelessWidget
{
  public SlidableDrawerActionPane(Key key = null) : base(key: key)
  {

  }


  public override Widget build(BuildContext context)
  {
    SlidableData data = SlidableData.of(context);

    var alignment = data.alignment;
    var startOffset = new Offset(alignment.x, alignment.y);
    var animations = Enumerable.Range(0, data.actionCount).Select(index => new OffsetTween(
      begin: startOffset,
      end: startOffset * (index - 1.0f)
    ).animate(data.actionsMoveAnimation)).ToList();

    return new _SlidableStackActionPane(
      data: data,
      child: Positioned.fill(
        child: new Stack(
          alignment: data.alignment,
          children: Enumerable.Range(0, data.actionCount).Select(
            (index) =>
            {
              int displayIndex =
                data.showActions ? data.actionCount - index - 1 : index;
              return new FractionallySizedBox(
                alignment: data.alignment,
                widthFactor:
                data.directionIsXAxis ? data.actionExtentRatio : null as float?,
                heightFactor:
                data.directionIsXAxis ? null as float? : data.actionExtentRatio,
                child: new SlideTransition(
                  position: animations[index],
                  child: data.actionDelegate.build(
                    context,
                    displayIndex,
                    data.actionsMoveAnimation,
                    SlidableRenderingMode.slide
                  )
                )
              ) as Widget;
            }).ToList()
        )
      )
    );
  }
}