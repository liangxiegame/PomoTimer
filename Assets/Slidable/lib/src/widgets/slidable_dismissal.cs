using System;
using System.Collections.Generic;
using System.Linq;
using QF.Slidable;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

/// A wiget that controls how the [Slidable] is dismissed.
///
/// The [Slidable] widget calls the [onDismissed] callback either after its size has
/// collapsed to zero (if [resizeDuration] is non-null) or immediately after
/// the slide animation (if [resizeDuration] is null). If the [Slidable] is a
/// list item, it must have a key that distinguishes it from the other items and
/// its [onDismissed] callback must remove the item from the list.
///
/// See also:
///
///  * [SlidableDrawerDismissal], which creates slide actions that are displayed like drawers
///  while the item is dismissing.
class SlidableDismissal : StatelessWidget
{
  private static readonly TimeSpan _kResizeDuration = TimeSpan.FromMilliseconds(300);

  public SlidableDismissal(
    Widget child = null,
    Dictionary<SlideActionType, float> dismissThresholds = null,
    VoidCallback onResize = null,
    DismissSlideActionCallback onDismissed = null,
    SlideActionWillBeDismissed onWillDismiss = null,
    TimeSpan? resizeDuration = null,
    double crossAxisEndOffset = 0.0f,
    bool closeOnCanceled = false
  )
  {
    D.assert(dismissThresholds != null);

    this.child = child;

    this.onResize = onResize;

    this.dismissThresholds = dismissThresholds ?? new Dictionary<SlideActionType, float>();
    this.onDismissed = onDismissed;
    this.onWillDismiss = onWillDismiss;

    this.resizeDuration = resizeDuration ?? _kResizeDuration;
    this.crossAxisEndOffset = crossAxisEndOffset;
    this.closeOnCanceled = closeOnCanceled;
  }

  /// The offset threshold the item has to be dragged in order to be considered
  /// dismissed.
  ///
  /// Represented as a fraction, e.g. if it is 0.4 (the default), then the item
  /// has to be dragged at least 40% towards one direction to be considered
  /// dismissed. Clients can define different thresholds for each dismiss
  /// direction.
  ///
  /// Flinging is treated as being equivalent to dragging almost to 1.0, so
  /// flinging can dismiss an item past any threshold less than 1.0.
  ///
  /// Setting a threshold of 1.0 (or greater) prevents a drag for
  //  the given [SlideActionType]
  public Dictionary<SlideActionType, float> dismissThresholds;

  /// Called when the widget has been dismissed, after finishing resizing.
  public DismissSlideActionCallback onDismissed;

  /// Called before the widget is dismissed. If the call returns false, the
  /// item will not be dismissed.
  ///
  /// If null, the widget will always be dismissed.
  public SlideActionWillBeDismissed onWillDismiss;

  /// Specifies to close this slidable after canceling dismiss.
  ///
  /// Defaults to false.
  public bool closeOnCanceled;

  /// Called when the widget changes size (i.e., when contracting before being dismissed).
  public VoidCallback onResize;

  /// The amount of time the widget will spend contracting before [onDismissed] is called.
  ///
  /// If null, the widget will not contract and [onDismissed] will be called
  /// immediately after the widget is dismissed.
  public TimeSpan resizeDuration;

  /// Defines the end offset across the main axis after the card is dismissed.
  ///
  /// If non-zero value is given then widget moves in cross direction depending on whether
  /// it is positive or negative.
  double crossAxisEndOffset;

  /// The widget to show when the [Slidable] enters dismiss mode.
  Widget child;

  public override Widget build(BuildContext context)
  {
    var data = SlidableData.of(context);

    return new AnimatedBuilder(
      animation: data.overallMoveAnimation,
      child: child,
      builder: (BuildContext context1, Widget child1) =>
      {
        if (data.overallMoveAnimation.value > data.totalActionsExtent)
        {
          return child1;
        }
        else
        {
          return data.slidable.actionPane;
        }
      }
    );
  }
}

/// A specific dismissal that creates slide actions that are displayed like drawers
/// while the item is dismissing.
/// The further slide action will grow faster than the other ones.
class SlidableDrawerDismissal : StatelessWidget
{
  public SlidableDrawerDismissal(Key key = null) : base(key: key)
  {
  }

  public override Widget build(BuildContext context)
  {
    SlidableData data = SlidableData.of(context);

    var animation = new OffsetTween(
      begin: Offset.zero,
      end: data.createOffset(data.actionSign)
    ).animate(data.overallMoveAnimation);

    var count = data.actionCount;

    var extentAnimations = Enumerable.Range(0, count).Select((index) =>
    {
      return new FloatTween(
        begin: data.actionExtentRatio,
        end: 1 - data.actionExtentRatio * (data.actionCount - index - 1)
      ).animate(
        new CurvedAnimation(
          parent: data.overallMoveAnimation,
          curve: new Interval(data.totalActionsExtent, 1.0f)
        )
      );
    }).ToList();

    return new Stack(
      children: new List<Widget>
      {
        new AnimatedBuilder(
          animation: data.overallMoveAnimation,
          builder: (context1, child) =>
          {
            return Positioned.fill(
              child: new Stack(
                children: Enumerable.Range(0, data.actionCount).Select((index) =>
                {
                  // For the main actions we have to reverse the order if we want the last item at the bottom of the stack.
                  int displayIndex =
                    data.showActions ? data.actionCount - index - 1 : index;

                  return data.createFractionallyAlignedSizedBox(
                    positionFactor: data.actionExtentRatio *
                                    (data.actionCount - index - 1),
                    extentFactor: extentAnimations[index].value,
                    child: data.actionDelegate.build(context1, displayIndex,
                      data.actionsMoveAnimation, data.renderingMode)) as Widget;
                }).ToList()
              ));
          }
          // return Stack(
          //   children: List.generate(data.actionCount, (index) {
          //     // For the main actions we have to reverse the order if we want the last item at the bottom of the stack.
          //     int displayIndex =
          //         data.showActions ? data.actionCount - index - 1 : index;

          //     return data.createFractionallyAlignedSizedBox(
          //       positionFactor:
          //           data.actionExtentRatio * (data.actionCount - index - 1),
          //       extentFactor: extentAnimations[index].value,
          //       child: data.actionDelegate.build(context, displayIndex,
          //           data.actionsMoveAnimation, data.renderingMode),
          //     );
          //   }),
          // );
        ),
        // Positioned.fill(
        //   child: LayoutBuilder(builder: (context, constraints) {
        //     final count = data.actionCount;
        //     final double totalExtent = data.getMaxExtent(constraints);
        //     final double actionExtent = totalExtent * data.actionExtentRatio;

        //     final extentAnimations = Iterable.generate(count).map((index) {
        //       return Tween<double>(
        //         begin: actionExtent,
        //         end: totalExtent -
        //             (actionExtent * (data.actionCount - index - 1)),
        //       ).animate(
        //         CurvedAnimation(
        //           parent: data.overallMoveAnimation,
        //           curve: Interval(data.totalActionsExtent, 1.0),
        //         ),
        //       );
        //     }).toList();

        //     return AnimatedBuilder(
        //         animation: data.overallMoveAnimation,
        //         builder: (context, child) {
        //           return Stack(
        //             children: List.generate(data.actionCount, (index) {
        //               // For the main actions we have to reverse the order if we want the last item at the bottom of the stack.
        //               int displayIndex = data.showActions
        //                   ? data.actionCount - index - 1
        //                   : index;
        //               return data.createPositioned(
        //                 position: actionExtent * (data.actionCount - index - 1),
        //                 extent: extentAnimations[index].value,
        //                 child: data.actionDelegate.build(context, displayIndex,
        //                     data.actionsMoveAnimation, data.renderingMode),
        //               );
        //             }),
        //           );
        //         });
        //   }),
        // ),
        new SlideTransition(
          position: animation,
          child: data.slidable.child
        )
      }
    );
  }
}