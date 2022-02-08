using ImageMeasurementApps;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ImageMeasurementApp
{
    public class TopRulerManager : HorizontalRulerManager
    {
        public TopRulerManager(RulerBase control) : base(control) { }

        public override Line CreateMajorLine(double offset)
        {
            var line = GetBaseLine();

            line.X1 = offset;
            line.Y1 = 0;
            
            line.X2 = offset;
            line.Y2 = GetHeight();
         
            return line;
        }

        public override Line CreateMinorLine(double offset)
        {
            var line = GetBaseLine();

            line.X1 = offset;
            line.Y1 = GetHeight() * (1 - Control.MinorStepRatio);
            
            line.X2 = offset;
            line.Y2 = GetHeight();

            return line;
        }

        public override TextBlock CreateText(double value, double offset)
        {
            var text = GetTextBlock(value.ToString(Control.TextFormat, GetTextCulture()));

            text.SetValue(Canvas.LeftProperty, offset);

            return text;
        }
    }
}
