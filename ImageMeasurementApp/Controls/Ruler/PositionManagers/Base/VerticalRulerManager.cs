using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageMeasurementApp
{
    public abstract class VerticalRulerManager : RulerPositionManager
    {
        protected VerticalRulerManager(RulerBase control) : base(control) { }

        public override double GetSize() => Control.ActualHeight;
        public override double GetHeight() => Control.ActualWidth;

        protected override void OnUpdateFirstStepControl(Canvas control, double stepSize)
        {
            control.VerticalAlignment = VerticalAlignment.Top;
            control.Height = stepSize;
        }

        protected override void OnUpdateStepRepeaterControl(System.Windows.Shapes.Rectangle control, VisualBrush brush, double stepSize)
        {
            brush.Viewport = new Rect(0, 0, GetHeight(), stepSize);
            control.Margin = new Thickness(0, stepSize, 0, 0);
        }

        protected override bool OnUpdateMakerPosition(Line marker, Point position)
        {
            if (position.Y <= 0 || position.Y >= GetSize())
                return false;

            marker.X1 = 0;
            marker.Y1 = position.Y;
            
            marker.X2 = GetHeight();
            marker.Y2 = position.Y;

            return true;
        }
    }
}
