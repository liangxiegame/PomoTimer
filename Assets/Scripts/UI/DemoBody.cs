using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;

namespace PomodoroApp.UI
{
    public class DemoBody : StatefulWidget
    {
        public Size Size;
        public int XOffset;
        public int YOffset;
        public Color Color;

        public DemoBody(Size size, Color color,int xOffset = 0, int yOffset= 0)
        {
            Size = size;
            XOffset = xOffset;
            YOffset = yOffset;
            Color = color;
        }


        public override State createState()
        {
            return new DemoBodyState();
        }
    }

    class DemoBodyState : TickerProviderStateMixin<DemoBody>
    {
        private AnimationController mAnimationController;
        List<Offset>                mAnimList1 = new List<Offset>();


        public override void initState()
        {
            base.initState();

            mAnimationController = new AnimationController(
                vsync: this,
                duration: TimeSpan.FromSeconds(2)
            );

            mAnimationController.addListener(() =>
            {
                mAnimList1.Clear();
                for (int i = -2 - widget.XOffset; i <= (int) widget.Size.width + 2; i++)
                {
                    mAnimList1.Add(new Offset(
                        i + widget.XOffset,
                        Mathf.Sin((mAnimationController.value * 360 - i) % 360 * Mathf.Deg2Rad) * 10 + 30 +
                        widget.YOffset));
                }
            });
            mAnimationController.repeat();
        }

        public override void dispose()
        {
            mAnimationController.dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context)
        {
            return new Container(
                alignment: Alignment.bottomCenter,
                child: new AnimatedBuilder(
                    animation: new CurvedAnimation(
                        parent: mAnimationController,
                        curve: Curves.easeInOut
                    ),
                    builder: (buildContext, child) =>
                    {
                        return new ClipPath(
                            child: new Container(
                                width: widget.Size.width,
                                height: widget.Size.height,
                                color: widget.Color
                            ),
                            clipper: new WaveClipper(mAnimationController.value, mAnimList1)
                        );
                    }
                )
            );
        }
    }

    class WaveClipper : CustomClipper<Path>
    {
        public float mAnimation;
        
        public List<Offset> mWaveList1 = new List<Offset>();

        public WaveClipper(float animation, List<Offset> waveList1)
        {
            mAnimation = animation;
            mWaveList1 = waveList1;
        }

        public override Path getClip(Size size)
        {
            var path = new Path();
            
            path.addPolygon(mWaveList1,false);
            
            path.lineTo(size.width,size.height);
            path.lineTo(0.0f,size.height);
            path.close();

            return path;
        }

        public override bool shouldReclip(CustomClipper<Path> oldClipper)
        {
            return mAnimation != (oldClipper as WaveClipper).mAnimation;
        }
    }
}