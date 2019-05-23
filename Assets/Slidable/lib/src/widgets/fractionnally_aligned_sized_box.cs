
using System.Runtime.InteropServices;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;

namespace QF.Slidable
{
  public class FractionallyAlignedSizedBox : StatelessWidget
  {
    public FractionallyAlignedSizedBox(
      Widget child,
      Key key = null,
      float? leftFactor = null,
      float? topFactor = null,
      float? rightFactor = null,
      float? bottomFactor = null,
      float? widthFactor = null,
      float? heightFactor = null
    ) : base(key: key)
    {
      D.assert(leftFactor == null || rightFactor == null || widthFactor == null);
      D.assert(topFactor == null || bottomFactor == null || heightFactor == null);
      D.assert(widthFactor == null || widthFactor >= 0.0);
      D.assert(heightFactor == null || heightFactor >= 0.0);

      this.child = child;
      this.leftFactor = leftFactor;
      this.topFactor = topFactor;
      this.rightFactor = rightFactor;
      this.bottomFactor = bottomFactor;
      this.widthFactor = widthFactor;
      this.heightFactor = heightFactor;
    }

    float? leftFactor;
    float? topFactor;
    float? rightFactor;
    float? bottomFactor;
    float? widthFactor;
    float? heightFactor;
    Widget child;

    public override Widget build(BuildContext context)
    {
      float dx = 0;
      float dy = 0;
      var width = widthFactor;
      var height = heightFactor;

      if (widthFactor == null)
      {
        var left = leftFactor ?? 0;
        var right = rightFactor ?? 0;
        width = 1 - left - right;

        if (width.HasValue && width.Value != 1f)
        {
          dx = left / (1.0f - width.Value);
        }
      }

      if (heightFactor == null)
      {
        var top = topFactor ?? 0;
        var bottom = bottomFactor ?? 0;
        height = 1 - top - bottom;
        if (height != 1)
        {
          dy = top / (1.0f - height.Value);
        }
      }

      if (widthFactor != null && widthFactor != 1)
      {
        if (leftFactor != null)
        {
          dx = leftFactor.Value / (1 - widthFactor.Value);
        }
        else if (leftFactor == null && rightFactor != null)
        {
          dx = (1 - widthFactor.Value - rightFactor.Value) / (1 - widthFactor.Value);
        }
      }

      if (heightFactor != null && heightFactor != 1)
      {
        if (topFactor != null)
        {
          dy = topFactor.Value / (1 - heightFactor.Value);
        }
        else if (topFactor == null && bottomFactor != null)
        {
          dy = (1 - heightFactor.Value - bottomFactor.Value) / (1 - heightFactor.Value);
        }
      }

      return new Align(
        alignment: new FractionalOffset(
          dx,
          dy
        ),
        child: new FractionallySizedBox(
          widthFactor: width,
          heightFactor: height,
          child: child
        )
      );
    }
  }
}