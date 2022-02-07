using System;
using System.Globalization;

namespace ImageMeasurementApp
{
    /// <summary>
    /// A converter that takes in a boolean and returns a thickness of 2 if true, useful for applying 
    /// border radius on a true value
    /// </summary>
    public class BooleanToBorderThicknessConverter : BaseValueConverter<BooleanToBorderThicknessConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (bool)value ? 1 : 0;
            else
                return (bool)value ? 0 : 1;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
