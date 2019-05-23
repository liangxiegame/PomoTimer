

using System;
using System.Collections.Generic;
using System.Linq;
using QF.Slidable;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;

namespace QF.Slidable
{
  public class Constants
  {
    public const           float    _kActionsExtentRatio = 0.25f;
    public const           float    _kFastThreshold      = 2500.0f;
    public const           float    _kDismissThreshold   = 0.75f;
    public static readonly Curve    _kResizeTimeCurve    = new Interval(0.4f, 1.0f, curve: Curves.ease);
    public static readonly TimeSpan _kMovementDuration   = TimeSpan.FromMilliseconds(200);
  }

  /// The rendering mode in which the [Slidable] is.
  enum SlidableRenderingMode
  {
    /// The [Slidable] is not showing actions.
    none,

    /// The [Slidable] is showing actions during sliding.
    slide,

    /// The [Slidable] is showing actions during dismissing.
    dismiss,

    /// The [Slidable] is resizing after dismissing.
    resize,
  }

  /// The type of slide action that is currently been showed by the [Slidable].
  enum SlideActionType
  {
    /// The [actions] are being shown.
    primary,

    /// The [secondaryActions] are being shown.
    secondary,
  }

  /// Signature used by [SlideToDismissDelegate] to indicate that it has been
  /// dismissed for the given [actionType].
  ///
  /// Used by [SlideToDismissDelegate.onDismissed].
  delegate void DismissSlideActionCallback(SlideActionType actionType);

  /// Signature for determining whether the widget will be dismissed for the
  /// given [actionType].
  ///
  /// Used by [SlideToDismissDelegate.onWillDismiss].
  delegate bool SlideActionWillBeDismissed(SlideActionType actionType);

  /// Signature for the builder callback used to create slide actions.
  delegate Widget SlideActionBuilder(BuildContext context, int index,
    Animation<float> animation, SlidableRenderingMode step);

  /// A delegate that supplies slide actions.
  ///
  /// It's uncommon to subclass [SlideActionDelegate]. Instead, consider using one
  /// of the existing subclasses that provide adaptors to builder callbacks or
  /// explicit action lists.
  ///
  /// See also:
  ///
  ///  * [SlideActionBuilderDelegate], which is a delegate that uses a builder
  ///    callback to construct the slide actions.
  ///  * [SlideActionListDelegate], which is a delegate that has an explicit list
  ///    of slidable action.
  abstract class SlideActionDelegate
  {
    /// Abstract const constructor. This constructor enables subclasses to provide
    /// const constructors so that they can be used in const expressions.

    /// Returns the child with the given index.
    ///
    /// Must not return null.
    public abstract Widget build(BuildContext context, int index, Animation<float> animation,
      SlidableRenderingMode step);

    /// Returns the number of actions this delegate will build.
    public int actionCount { get; }
  }

  /// A delegate that supplies slide actions using a builder callback.
  ///
  /// This delegate provides slide actions using a [SlideActionBuilder] callback,
  /// so that the animation can be passed down to the final widget.
  ///
  /// See also:
  ///
  ///  * [SlideActionListDelegate], which is a delegate that has an explicit list
  ///    of slide action.
  class SlideActionBuilderDelegate : SlideActionDelegate
  {
    /// Creates a delegate that supplies slide actions using the given
    /// builder callback.
    ///
    /// The [builder] must not be null. The [actionCount] argument must not be positive.

    public SlideActionBuilderDelegate(SlideActionBuilder builder, int? actionCount = null)
    {
      D.assert(actionCount != null && actionCount > 0);
      this.builder = builder;
      this.actionCount = this.actionCount;
    }

    /// Called to build slide actions.
    ///
    /// Will be called only for indices greater than or equal to zero and less
    /// than [childCount].
    SlideActionBuilder builder;

    /// The total number of slide actions this delegate can provide.
    int actionCount;


    public override Widget build(BuildContext context, int index, Animation<float> animation,
      SlidableRenderingMode step) =>
      builder(context, index, animation, step);
  }

  /// A delegate that supplies slide actions using an explicit list.
  ///
  /// This delegate provides slide actions using an explicit list,
  /// which is convenient but reduces the benefit of passing the animation down
  /// to the final widget.
  ///
  /// See also:
  ///
  ///  * [SlideActionBuilderDelegate], which is a delegate that uses a builder
  ///    callback to construct the slide actions.
  class SlideActionListDelegate : SlideActionDelegate
  {
    /// Creates a delegate that supplies slide actions using the given
    /// list.
    ///
    /// The [actions] argument must not be null.
    public SlideActionListDelegate(List<Widget> actions)
    {
      this.actions = actions;
    }

    /// The slide actions.
    List<Widget> actions;


    int actionCount => actions?.Count ?? 0;


    public override Widget build(BuildContext context, int index, Animation<float> animation,
      SlidableRenderingMode step) =>
      actions[index];
  }

  class _SlidableScope : InheritedWidget
  {

    public _SlidableScope(
      Key key = null,
      SlidableState state = null,
      Widget child = null
    ) : base(key: key, child: child)
    {
      this.state = state;
    }

    public SlidableState state;

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
      return (oldWidget as _SlidableScope).state != state;
    }
  }

  class SlidableData : InheritedWidget
  {


    public SlidableData(
      float totalActionsExtent,
      float dismissThreshold,
      float actionExtentRatio,
      Axis direction,
      bool dismissible,
      SlideActionDelegate actionDelegate = null,
      Animation<float> overallMoveAnimation = null,
      Animation<float> actionsMoveAnimation = null,
      Animation<float> dismissAnimation = null,
      Slidable slidable = null,
      Key key = null,
      SlideActionType actionType = SlideActionType.primary,
      SlidableRenderingMode renderingMode = SlidableRenderingMode.none,
      Widget child = null
    ) : base(key: key, child: child)
    {
      this.totalActionsExtent = totalActionsExtent;
      this.dismissThreshold = dismissThreshold;
      this.actionExtentRatio = actionExtentRatio;
      this.direction = direction;
      this.dismissible = dismissible;
      this.actionDelegate = actionDelegate;
      this.overallMoveAnimation = overallMoveAnimation;
      this.actionsMoveAnimation = actionsMoveAnimation;
      this.dismissAnimation = dismissAnimation;
      this.slidable = slidable;
      this.actionType = actionType;
      this.renderingMode = renderingMode;
    }

    SlideActionType              actionType;
    public SlidableRenderingMode renderingMode;
    public float                 totalActionsExtent;
    float                        dismissThreshold;
    bool                         dismissible;

    /// The current actions that have to be shown.
    public SlideActionDelegate actionDelegate;

    public Animation<float> overallMoveAnimation;
    public Animation<float> actionsMoveAnimation;
    Animation<float>        dismissAnimation;
    public Slidable         slidable;

    /// Relative ratio between one slide action and the extent of the child.
    public float actionExtentRatio;

    /// The direction in which this widget can be slid.
    public Axis direction;

    public bool  showActions      => actionType == SlideActionType.primary;
    public int   actionCount      => actionDelegate?.actionCount ?? 0;
    public float actionSign       => actionType == SlideActionType.primary ? 1.0f : -1.0f;
    public bool  directionIsXAxis => direction == Axis.horizontal;

    public Alignment alignment => new Alignment(
      directionIsXAxis ? -actionSign : 0.0f,
      directionIsXAxis ? 0.0f : -actionSign
    );

    public float? actionPaneWidthFactor =>
      directionIsXAxis ? totalActionsExtent : null as float?;

    public float? actionPaneHeightFactor =>
      directionIsXAxis ? null as float? : totalActionsExtent;

    /// The data from the closest instance of this class that encloses the given context.
    public static SlidableData of(BuildContext context)
    {
      return context.inheritFromWidgetOfExactType(typeof(SlidableData)) as SlidableData;
    }

    /// Gets the the given offset related to the current direction.
    public Offset createOffset(float value)
    {
      return directionIsXAxis ? new Offset(value, 0.0f) : new Offset(0.0f, value);
    }

    /// Gets the maximum extent in the current direction.
    float getMaxExtent(BoxConstraints constraints)
    {
      return directionIsXAxis ? constraints.maxWidth : constraints.maxHeight;
    }

    /// Creates a positioned related to the current direction and showed actions.
    Positioned createPositioned(
      Widget child,
      float extent,
      float position
    )
    {
      return new Positioned(
        left: directionIsXAxis ? (showActions ? position : null as float?) : 0.0f,
        right: directionIsXAxis ? (showActions ? null as float? : position) : 0.0f,
        top: directionIsXAxis ? 0.0f : (showActions ? position : null as float?),
        bottom: directionIsXAxis ? 0.0f : (showActions ? null as float? : position),
        width: directionIsXAxis ? extent : null as float?,
        height: directionIsXAxis ? null as float? : extent,
        child: child
      );
    }

    public FractionallyAlignedSizedBox createFractionallyAlignedSizedBox(
      Widget child,
      float? extentFactor,
      float? positionFactor
    )
    {
      return new FractionallyAlignedSizedBox(
        leftFactor:
        directionIsXAxis ? (showActions ? positionFactor : null) : 0.0f,
        rightFactor:
        directionIsXAxis ? (showActions ? null : positionFactor) : 0.0f,
        topFactor: directionIsXAxis ? 0.0f : showActions ? positionFactor : null,
        bottomFactor:
        directionIsXAxis ? 0.0f : (showActions ? null : positionFactor),
        widthFactor: directionIsXAxis ? extentFactor : null,
        heightFactor: directionIsXAxis ? null : extentFactor,
        child: child
      );
    }

    /// Builds the slide actions using the active [SlideActionDelegate]'s builder.
    public List<Widget> buildActions(BuildContext context)
    {
      return Enumerable.Range(0, actionCount).Select(
        index => actionDelegate.build(
          context,
          index,
          actionsMoveAnimation,
          SlidableRenderingMode.slide
        )).ToList();
    }


    public override bool updateShouldNotify(InheritedWidget data)
    {
      var oldWidget = data as SlidableData;
      return oldWidget.actionType != actionType ||
             oldWidget.renderingMode != renderingMode ||
             oldWidget.totalActionsExtent != totalActionsExtent ||
             oldWidget.dismissThreshold != dismissThreshold ||
             oldWidget.dismissible != dismissible ||
             oldWidget.actionDelegate != actionDelegate ||
             oldWidget.overallMoveAnimation != overallMoveAnimation ||
             oldWidget.actionsMoveAnimation != actionsMoveAnimation ||
             oldWidget.dismissAnimation != dismissAnimation ||
             oldWidget.slidable != slidable ||
             oldWidget.actionExtentRatio != actionExtentRatio ||
             oldWidget.direction != direction;
    }
  }

  /// A controller that keep tracks of the active [SlidableState] and close
  /// the previous one.
  class SlidableController
  {
    public SlidableController(ValueChanged<Animation<float>> onSlideAnimationChanged,
      ValueChanged<bool> onSlideIsOpenChanged)
    {
      this.onSlideAnimationChanged = onSlideAnimationChanged;
      this.onSlideIsOpenChanged = onSlideIsOpenChanged;
    }

    ValueChanged<Animation<float>> onSlideAnimationChanged;
    ValueChanged<bool>             onSlideIsOpenChanged;
    bool                           _isSlideOpen;

    Animation<float> _slideAnimation;

    public SlidableState _activeState;

    public SlidableState activeState
    {
      get => _activeState;

      set
      {
        _activeState?._flingAnimationController();

        _activeState = value;
        if (onSlideAnimationChanged != null)
        {
          _slideAnimation?.removeListener(_handleSlideIsOpenChanged);
          if (onSlideIsOpenChanged != null)
          {
            _slideAnimation = value?.overallMoveAnimation;
            _slideAnimation?.addListener(_handleSlideIsOpenChanged);
            if (_slideAnimation == null)
            {
              _isSlideOpen = false;
              onSlideIsOpenChanged(_isSlideOpen);
            }
          }

          onSlideAnimationChanged(value?.overallMoveAnimation);
        }
      }
    }

    void _handleSlideIsOpenChanged()
    {
      if (onSlideIsOpenChanged != null && _slideAnimation != null)
      {
        bool isOpen = _slideAnimation.value != 0.0;
        if (isOpen != _isSlideOpen)
        {
          _isSlideOpen = isOpen;
          onSlideIsOpenChanged(_isSlideOpen);
        }
      }
    }
  }

  /// A widget that can be slid in both direction of the specified axis.
  ///
  /// If the direction is [Axis.horizontal], this widget can be slid to the left or to the right,
  /// otherwise this widget can be slid up or slid down.
  ///
  /// By sliding in one of these direction, slide actions will appear.
  class Slidable : StatefulWidget
  {
    /// Creates a widget that can be slid.
    ///
    /// The [actions] contains the slide actions that appears when the child has been dragged down or to the right.
    /// The [secondaryActions] contains the slide actions that appears when the child has been dragged up or to the left.
    ///
    /// The [delegate] and [closeOnScroll] arguments must not be null. The [actionExtentRatio]
    /// and [showAllActionsThreshold] arguments must be greater or equal than 0 and less or equal than 1.
    ///
    /// The [key] argument must not be null if the `slideToDismissDelegate`
    /// is provided because [Slidable]s are commonly
    /// used in lists and removed from the list when dismissed. Without keys, the
    /// default behavior is to sync widgets based on their index in the list,
    /// which means the item after the dismissed item would be synced with the
    /// state of the dismissed item. Using keys causes the widgets to sync
    /// according to their keys and avoids this pitfall.
    public Slidable(
      Key key = null,
      Widget child = null,
      SlidableDelegate slidableDelegate = null,
      Widget actionPane = null,
      List<Widget> actions = null,
      List<Widget> secondaryActions = null,
      SlidableDismissal dismissal = null,
      SlidableController controller = null,

      float? fastThreshold = null,
      TimeSpan? movementDuration = null,
      float showAllActionsThreshold = 0.5f,
      float actionExtentRatio = Constants._kActionsExtentRatio,
      Axis direction = Axis.horizontal,
      bool closeOnScroll = true,
      bool enabled = true
    ) : this(
      key,
      child,
      slidableDelegate,
      actionPane,
      new SlideActionListDelegate(actions: actions),
      new SlideActionListDelegate(actions: secondaryActions),
      showAllActionsThreshold: showAllActionsThreshold,
      actionExtentRatio: actionExtentRatio,
      movementDuration: movementDuration ?? Constants._kMovementDuration,
      direction: direction,
      closeOnScroll: closeOnScroll,
      enabled: enabled,
      dismissal: dismissal,
      controller: controller,
      fastThreshold: fastThreshold,
      
    )
    {
    }

    private SlidableController Controller;

    /// Creates a widget that can be slid.
    ///
    /// The [actionDelegate] is a delegate that builds the slide actions that appears when the child has been dragged down or to the right.
    /// The [secondaryActionDelegate] is a delegate that builds the slide actions that appears when the child has been dragged up or to the left.
    ///
    /// The [delegate], [closeOnScroll] and [enabled] arguments must not be null. The [actionExtentRatio]
    /// and [showAllActionsThreshold] arguments must be greater or equal than 0 and less or equal than 1.
    ///
    /// The [key] argument must not be null if the `slideToDismissDelegate`
    /// is provided because [Slidable]s are commonly
    /// used in lists and removed from the list when dismissed. Without keys, the
    /// default behavior is to sync widgets based on their index in the list,
    /// which means the item after the dismissed item would be synced with the
    /// state of the dismissed item. Using keys causes the widgets to sync
    /// according to their keys and avoids this pitfall.
    public Slidable(
      Key key,
      Widget child,
      SlidableDelegate slidableDelegate,
      Widget actionPane,
      SlideActionDelegate actionDelegate,
      SlideActionDelegate secondaryActionDelegate,
      TimeSpan movementDuration,
      float? fastThreshold,
      SlidableDismissal dismissal,
      SlidableController controller,
      float showAllActionsThreshold = 0.5f,
      float actionExtentRatio = Constants._kActionsExtentRatio,
      Axis direction = Axis.horizontal,
      bool closeOnScroll = true,
      bool enabled = true
    ) :

      base(key: key)
    {

      D.assert(actionPane != null);
      D.assert(direction != null);
      D.assert(
        showAllActionsThreshold != null &&
        showAllActionsThreshold >= 0.0f &&
        showAllActionsThreshold <= 1.0f,
        () => "showAllActionsThreshold must be between 0.0 and 1.0");

      D.assert(
        actionExtentRatio != null &&
        actionExtentRatio >= 0.0f &&
        actionExtentRatio <= 1.0f,
        () => "actionExtentRatio must be between 0.0 and 1.0");

      D.assert(closeOnScroll != null);
      D.assert(enabled != null);
      D.assert(dismissal == null || key != null,
        () => "a key must be provided if slideToDismissDelegate is set");
      D.assert(fastThreshold == null || fastThreshold >= .0,
        () => "fastThreshold must be positive");
      this.fastThreshold = fastThreshold ?? Constants._kFastThreshold;

      this.child = child;
      this.actionPane = actionPane;
      this.actionDelegate = actionDelegate;
      this.secondaryActionDelegate = secondaryActionDelegate;
      this.movementDuration = movementDuration;
      this.fastThreshold = fastThreshold != null ? fastThreshold.Value : this.fastThreshold;
      this.dismissal = dismissal;
      this.controller = controller;
      this.showAllActionsThreshold = showAllActionsThreshold;
      this.actionExtentRatio = actionExtentRatio;
      this.direction = direction;
      this.closeOnScroll = closeOnScroll;
      this.enabled = enabled;
    }

    /// The widget below this widget in the tree.
    public Widget child;

    /// The controller that tracks the active [Slidable] and keep only one open.
    public SlidableController controller;

    /// A delegate that builds slide actions that appears when the child has been dragged
    /// down or to the right.
    public SlideActionDelegate actionDelegate;

    /// A delegate that builds slide actions that appears when the child has been dragged
    /// up or to the left.
    public SlideActionDelegate secondaryActionDelegate;

    /// The action pane that controls how the slide actions are animated;
    public Widget actionPane;

    /// A delegate that controls how to dismiss the item.
    public SlidableDismissal dismissal;

    /// Relative ratio between one slide action and the extent of the child.
    public float actionExtentRatio;

    /// The direction in which this widget can be slid.
    public Axis direction;

    /// The offset threshold the item has to be dragged in order to show all actions
    /// in the slide direction.
    ///
    /// Represented as a fraction, e.g. if it is 0.4 (the default), then the item
    /// has to be dragged at least 40% of the slide actions extent towards one direction to show all actions.
    public float showAllActionsThreshold;

    /// Defines the duration for card to go to final position or to come back to original position if threshold not reached.
    public TimeSpan movementDuration;

    /// Specifies to close this slidable after the closest [Scrollable]'s position changed.
    ///
    /// Defaults to true.
    public bool closeOnScroll;

    /// Whether this slidable is interactive.
    ///
    /// If false, the child will not slid to show slide actions.
    ///
    /// Defaults to true.
    public bool enabled;

    /// The threshold used to know if a movement was fast and request to open/close the actions.
    public float fastThreshold;

    /// The state from the closest instance of this class that encloses the given context.
    public static SlidableState of(BuildContext context)
    {
      _SlidableScope scope =
        context.inheritFromWidgetOfExactType(typeof(_SlidableScope)) as _SlidableScope;
      return scope?.state;
    }

    public override State createState()
    {
      return new SlidableState();
    }
  }

  /// The state of [Slidable] widget.
  /// You can open or close the [Slidable] by calling the corresponding methods of
  /// this object.
  class SlidableState : AutomaticKeepAliveClientWithTickerProviderStateMixin<Slidable>
  {
    public override void initState()
    {
      base.initState();
      _overallMoveController =
        new AnimationController(duration: widget.movementDuration, vsync: this);
      _overallMoveController.addStatusListener(_handleDismissStatusChanged);
      _overallMoveController.addListener(_handleOverallPositionChanged);
      _initAnimations();
    }

    void _initAnimations()
    {
      _actionsMoveAnimation
        ?.removeStatusListener(_handleShowAllActionsStatusChanged);
      _dismissAnimation?.removeStatusListener(_handleShowAllActionsStatusChanged);

      _actionsMoveAnimation = new CurvedAnimation(
        parent: _overallMoveController,
        curve: new Interval(0.0f, _totalActionsExtent)
      );
      _actionsMoveAnimation.addStatusListener(_handleShowAllActionsStatusChanged);

      _dismissAnimation = new CurvedAnimation(
        parent: _overallMoveController,
        curve: new Interval(_totalActionsExtent, 1.0f)
      );

      _dismissAnimation.addStatusListener(_handleShowAllActionsStatusChanged);
    }

    AnimationController     _overallMoveController;
    public Animation<float> overallMoveAnimation => _overallMoveController.view;

    Animation<float> _actionsMoveAnimation;
    Animation<float> _dismissAnimation;

    AnimationController _resizeController;
    Animation<float>    _resizeAnimation;

    float _dragExtent = 0.0f;

    SlidableRenderingMode _renderingMode = SlidableRenderingMode.none;
    SlidableRenderingMode renderingMode => _renderingMode;

    ScrollPosition _scrollPosition;
    bool           _dragUnderway = false;
    Size           _sizePriorToCollapse;
    bool           _dismissing = false;

    SlideActionType _actionType = SlideActionType.primary;

    private SlideActionType actionType
    {
      get => _actionType;
      set
      {
        _actionType = value;
        _initAnimations();
      }
    }

    int actionCount => actionDelegate?.actionCount ?? 0;

    float _totalActionsExtent => widget.actionExtentRatio * (actionCount);

    private float _dismissThreshold
    {
      get
      {

        if (widget.dismissal != null && widget.dismissal.dismissThresholds.ContainsKey(actionType))
        {

          return widget.dismissal.dismissThresholds[actionType];
        }

        return Constants._kDismissThreshold;
      }
    }

    bool _dismissible => widget.dismissal != null && _dismissThreshold < 1.0;


    protected override bool wantKeepAlive =>
      !widget.closeOnScroll &&
      (_overallMoveController?.isAnimating == true ||
       _resizeController?.isAnimating == true);

    /// The current actions that have to be shown.
    SlideActionDelegate actionDelegate =>
      actionType == SlideActionType.primary
        ? widget.actionDelegate
        : widget.secondaryActionDelegate;

    bool _directionIsXAxis => widget.direction == Axis.horizontal;

    float _overallDragAxisExtent
    {
      get
      {
        Size size = context.size;
        return _directionIsXAxis ? size.width : size.height;
      }
    }

    float _actionsDragAxisExtent => _overallDragAxisExtent * _totalActionsExtent;


    public override void didChangeDependencies()
    {
      base.didChangeDependencies();
      _removeScrollingNotifierListener();
      _addScrollingNotifierListener();
    }

    public override void didUpdateWidget(StatefulWidget oldWidget)
    {
      base.didUpdateWidget(oldWidget);

      if (widget.closeOnScroll != (oldWidget as Slidable).closeOnScroll)
      {
        _removeScrollingNotifierListener();
        _addScrollingNotifierListener();
      }
    }

    void _addScrollingNotifierListener()
    {
      if (widget.closeOnScroll)
      {
        _scrollPosition = Scrollable.of(context)?.position;
        _scrollPosition?.isScrollingNotifier.addListener(_isScrollingListener);
      }
    }

    void _removeScrollingNotifierListener()
    {
      _scrollPosition?.isScrollingNotifier.removeListener(_isScrollingListener);
    }

    public override void dispose()
    {
      _overallMoveController.dispose();
      _resizeController?.dispose();
      _removeScrollingNotifierListener();
      if (widget.controller != null)
        widget.controller._activeState = null;
      base.dispose();
    }

    /// Opens the [Slidable].
    /// By default it's open the [SlideActionType.primary] action pane, but you
    /// can modify this by setting [actionType].
    public void open(SlideActionType? actionType = null)
    {

      if (widget.controller != null)
        widget.controller.activeState = this;

      if (_actionType != actionType)
      {
        setState(() => { this.actionType = SlideActionType.primary; });
      }

      if (actionCount > 0)
      {
        _overallMoveController.animateTo(
          _totalActionsExtent,
          curve: Curves.easeIn,
          duration: widget.movementDuration
        );
      }
    }

    /// Closes this [Slidable].
    public void close()
    {
      if (!_overallMoveController.isDismissed)
      {
        if (widget.controller?.activeState == this)
        {
          if (widget.controller != null)
          {
            widget.controller.activeState = null;
          }
        }
        else
        {
          _flingAnimationController();
        }
      }
    }

    public void _flingAnimationController()
    {
      if (!_dismissing)
      {
        _overallMoveController.fling(velocity: -1.0f);
      }
    }

    /// Dismisses this [Slidable].
    /// By default it's dismiss by showing the [SlideActionType.primary] action pane, but you
    /// can modify this by setting [actionType].
    public void dismiss(SlideActionType? actionType = null)
    {
      if (_dismissible)
      {
        _dismissing = true;

        if (actionType == null)
        {
          actionType = _actionType;
        }

        if (actionType != _actionType)
        {
          setState(() => { this.actionType = actionType.Value; });
        }

        _overallMoveController.fling(velocity: 1.0f);
      }
    }

    void _isScrollingListener()
    {
      if (!widget.closeOnScroll || _scrollPosition == null) return;

      // When a scroll starts close this.
      if (_scrollPosition.isScrollingNotifier.value)
      {
        close();
      }
    }

    void _handleDragStart(DragStartDetails details)
    {
      
      Debug.Log("handle drag start");
      _dragUnderway = true;

      if (widget.controller != null)
      {
        widget.controller.activeState = this;
      }

      _dragExtent = _actionsMoveAnimation.value * _actionsDragAxisExtent * Mathf.Sign(_dragExtent);
      if (_overallMoveController.isAnimating)
      {
        _overallMoveController.stop();
      }
    }

    void _handleDragUpdate(DragUpdateDetails details)
    {
      if (widget.controller != null && widget.controller.activeState != this)
      {
        return;
      }

      float delta = details.primaryDelta.Value;
      _dragExtent += delta;
      setState(() =>
      {
        actionType = Mathf.Sign(_dragExtent) >= 0
          ? SlideActionType.primary
          : SlideActionType.secondary;
        if (actionCount > 0)
        {
          _overallMoveController.setValue(
            _dragExtent.abs() / _overallDragAxisExtent);
        }
      });
    }

    void _handleDragEnd(DragEndDetails details)
    {
      if (widget.controller != null && widget.controller.activeState != this)
      {
        return;
      }

      _dragUnderway = false;
      var velocity = details.primaryVelocity;
      bool shouldOpen = Mathf.Sign(velocity.Value) == Mathf.Sign(_dragExtent);
      bool fast = Mathf.Abs(velocity.Value) > widget.fastThreshold;

      if (_dismissible && overallMoveAnimation.value > _totalActionsExtent)
      {
        // We are in a dismiss state.
        if (overallMoveAnimation.value >= _dismissThreshold)
        {
          dismiss();
        }
        else
        {
          open(null);
        }
      }
      else if (_actionsMoveAnimation.value >= widget.showAllActionsThreshold ||
               (shouldOpen && fast))
      {
        open();
      }
      else
      {
        close();
      }
    }

    void _handleShowAllActionsStatusChanged(AnimationStatus status)
    {
      // Make sure to rebuild a last time, otherwise the slide action could
      // be scrambled.
      if (status == AnimationStatus.completed ||
          status == AnimationStatus.dismissed)
      {
        setState(() => { });
      }

      updateKeepAlive();
    }

    void _handleOverallPositionChanged()
    {
      float value = _overallMoveController.value;
      if (value == _overallMoveController.lowerBound)
      {
        _renderingMode = SlidableRenderingMode.none;
      }
      else if (value <= _totalActionsExtent)
      {
        _renderingMode = SlidableRenderingMode.slide;
      }
      else
      {
        _renderingMode = SlidableRenderingMode.dismiss;
      }

      setState(() => { });
    }

    void _handleDismissStatusChanged(AnimationStatus status)
    {
      if (_dismissible)
      {
        if (status == AnimationStatus.completed &&
            _overallMoveController.value == _overallMoveController.upperBound &&
            !_dragUnderway)
        {

          if (widget.dismissal.onWillDismiss == null ||
              widget.dismissal.onWillDismiss(actionType))
          {
            _startResizeAnimation();
          }
          else
          {
            _dismissing = false;
            if (widget.dismissal?.closeOnCanceled == true)
            {
              close();
            }
            else
            {
              open();
            }
          }
        }

        updateKeepAlive();
      }
    }

    void _handleDismiss()
    {
      if (widget.controller != null)
      {
        widget.controller.activeState = null;
      }

      SlidableDismissal dismissal = widget.dismissal;

      if (dismissal.onDismissed != null)
      {
        D.assert(actionType != null);
        dismissal.onDismissed(actionType);
      }
    }

    void _startResizeAnimation()
    {
      D.assert(_overallMoveController != null);
      D.assert(_overallMoveController.isCompleted);
      D.assert(_resizeController == null);
      D.assert(_sizePriorToCollapse == null);
      SlidableDismissal dismissal = widget.dismissal;
      if (dismissal.resizeDuration == null)
      {
        _handleDismiss();
      }
      else
      {
        _resizeController =
          new AnimationController(duration: dismissal.resizeDuration, vsync: this);
        _resizeController.addListener(_handleResizeProgressChanged);
        _resizeController.addStatusListener((AnimationStatus status) => updateKeepAlive());
        _resizeController.forward();

        setState(() =>
        {
          _renderingMode = SlidableRenderingMode.resize;
          _sizePriorToCollapse = context.size;
          _resizeAnimation = new FloatTween(begin: 1.0f, end: 0.0f).animate(
            new CurvedAnimation(
              parent: _resizeController, curve: Constants._kResizeTimeCurve));
        });
      }
    }

    void _handleResizeProgressChanged()
    {
      if (_resizeController.isCompleted)
      {
        _handleDismiss();
      }
      else
      {
        widget.dismissal.onResize?.Invoke();
      }
    }

    public override Widget build(BuildContext context)
    {
      base.build(context); // See AutomaticKeepAliveClientMixin.
      
      Debug.Log(widget.enabled);
      Debug.Log(widget.actionDelegate);
      Debug.Log(widget.actionDelegate.actionCount);
      Debug.Log(widget.secondaryActionDelegate);
      Debug.Log(widget.secondaryActionDelegate.actionCount);

      if (!widget.enabled ||
          (widget.actionDelegate == null ||
           widget.actionDelegate.actionCount == 0) &&
          (widget.secondaryActionDelegate == null ||
           widget.secondaryActionDelegate.actionCount == 0))
      {
        Debug.Log("widget.child @@@@");
        return widget.child;
      }

      var content = widget.child;


      Debug.Log("@@@@ 1");
      if (actionType == SlideActionType.primary &&
          widget.actionDelegate != null &&
          widget.actionDelegate.actionCount > 0 ||
          actionType == SlideActionType.secondary &&
          widget.secondaryActionDelegate != null &&
          widget.secondaryActionDelegate.actionCount > 0)
      {
        if (_dismissible)
        {
          content = widget.dismissal;

          if (_resizeAnimation != null)
          {
            // we've been dragged aside, and are now resizing.
            D.assert(() =>
            {
              if (_resizeAnimation.status != AnimationStatus.forward)
              {
                D.assert(_resizeAnimation.status == AnimationStatus.completed);
                throw new UIWidgetsError(
                  "A dismissed Slidable widget is still part of the tree.\nMake sure to implement the onDismissed handler and to immediately remove the Slidable\nwidget from the application once that handler has fired.");
              }

              return true;
            });

            content = new SizeTransition(
              sizeFactor: _resizeAnimation,
              axis: _directionIsXAxis ? Axis.vertical : Axis.horizontal,
              child: new SizedBox(
                width: _sizePriorToCollapse.width,
                height: _sizePriorToCollapse.height,
                child: content
              )
            );
          }
        }
        else
        {
          content = widget.actionPane;
        }
      }

      Debug.Log(_directionIsXAxis);


      content = new GestureDetector(
        onHorizontalDragStart: _directionIsXAxis ? new GestureDragStartCallback(_handleDragStart) : null,
        onHorizontalDragUpdate: _directionIsXAxis ? new GestureDragUpdateCallback(_handleDragUpdate) : null,
        onHorizontalDragEnd: _directionIsXAxis ? new GestureDragEndCallback(_handleDragEnd) : null,
        onVerticalDragStart: _directionIsXAxis ? null : new GestureDragStartCallback(_handleDragStart),
        onVerticalDragUpdate: _directionIsXAxis ? null : new GestureDragUpdateCallback(_handleDragUpdate),
        onVerticalDragEnd: _directionIsXAxis ? null : new GestureDragEndCallback(_handleDragEnd),
        behavior: HitTestBehavior.opaque,
        child: content
      );

      Debug.Log(_directionIsXAxis);

      return new _SlidableScope(
        state: this,
        child: new SlidableData(
          actionType: actionType,
          renderingMode: _renderingMode,
          totalActionsExtent: _totalActionsExtent,
          dismissThreshold: _dismissThreshold,
          dismissible: _dismissible,
          actionDelegate: actionDelegate,
          overallMoveAnimation: overallMoveAnimation,
          actionsMoveAnimation: _actionsMoveAnimation,
          dismissAnimation: _dismissAnimation,
          slidable: widget,
          actionExtentRatio: widget.actionExtentRatio,
          direction: widget.direction,
          child: content
        )
      );
    }
  }
}