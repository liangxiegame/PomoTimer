using System;
using System.Collections.Generic;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;

namespace PomoTimerApp
{
    public class WaveAnimation : StatefulWidget
    {
        public Size  size    { get; }
        public Color color   { get; }
        public int xOffset { get; }
        public int yOffset { get; }

        public WaveAnimation(Size size, Color color, int xOffset = 0, int yOffset = 0)
        {
            this.size = size;
            this.color = color;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }

        public override State createState()
        {
            return new WaveAnimationState();
        }
    }

    class WaveAnimationState : SingleTickerProviderStateMixin<WaveAnimation>
    {
        private AnimationController mController;
        private List<Offset>        mAnimList = new List<Offset>();

        public override void initState()
        {
            base.initState();

            mController = new AnimationController(
                duration: TimeSpan.FromSeconds(2),
                vsync: this
            );

            mController.addListener((() =>
            {
                mAnimList.Clear();

                for (var i = -2 - widget.xOffset; i < (int) widget.size.width + 2; i++)
                {
                    mAnimList.Add(new Offset(
                        i + widget.xOffset,
                        Mathf.Sin((mController.value * 360 - i) % 360 * Mathf.Deg2Rad) * 10 + 30 + widget.yOffset
                    ));
                }

            }));


            mController.repeat();
        }

        public override void dispose()
        {
            mController.dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context)
        {
            return new AnimatedBuilder(
                animation: new CurvedAnimation(
                    mController,
                    Curves.easeInOut
                ),
                builder: (buildContext, child) =>
                {
                    return new ClipPath(
                        child: new Container(
                            color: widget.color,
                            height: widget.size.height,
                            width: widget.size.width
                        ),
                        clipper: new WaveClipper(mController.value, mAnimList));
                });
        }
    }

    class WaveClipper : CustomClipper<Path>
    {
        public float controllerValue { get; }
        public List<Offset> animList { get; }

        public WaveClipper(float controllerValue, List<Offset> animList)
        {
            this.controllerValue = controllerValue;
            this.animList = animList;
        }

        public override Path getClip(Size size)
        {
            var path = new Path();
            
            path.addPolygon(animList,false);
            path.lineTo(size.width,size.height);
            path.lineTo(0.0f,size.height);
            
            path.close();

            return path;
        }

        public override bool shouldReclip(CustomClipper<Path> oldClipper)
        {
            return (oldClipper as WaveClipper).controllerValue != controllerValue;
        }
    }
}