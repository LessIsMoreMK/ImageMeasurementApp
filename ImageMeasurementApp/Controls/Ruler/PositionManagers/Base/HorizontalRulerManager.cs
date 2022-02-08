﻿using ImageMeasurementApp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageMeasurementApps
{
    public abstract class HorizontalRulerManager : RulerPositionManager
    {
        protected HorizontalRulerManager(RulerBase control) : base(control)  { }

        public override double GetSize() => Control.ActualWidth;
        public override double GetHeight() => Control.ActualHeight;

        protected override void OnUpdateFirstStepControl(Canvas control, double stepSize)
        {
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.Width = stepSize;
        }

        protected override void OnUpdateStepRepeaterControl(System.Windows.Shapes.Rectangle control, VisualBrush brush, double stepSize)
        {
            brush.Viewport = new Rect(0, 0, stepSize, GetHeight());
            control.Margin = new Thickness(stepSize, 0, 0, 0);
        }

        protected override bool OnUpdateMakerPosition(Line marker, Point position)
        {
            if (position.X <= 0 || position.X >= GetSize())
                return false;

            marker.X1 = position.X;
            marker.Y1 = 0;
            
            marker.X2 = position.X;
            marker.Y2 = GetHeight();

            return true;
        }
    }
}