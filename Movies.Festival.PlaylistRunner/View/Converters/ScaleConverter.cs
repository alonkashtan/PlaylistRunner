using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ak.oss.PlaylistRunner.View.Converters
{
    /// <summary>
    /// Moves a double between scales.
    /// For input [a, b, c], moves a from scale 0-b to scale 0-c, i.e. c*(a/b)
    /// <example>
    ///     Given [30, 100, 10] will return 3.
    /// </example>
    /// </summary>
    class ScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3 || !values.All(value => value is double))
                throw new ArgumentException("ScaleConverter supports only 3 doubles, in which the second is non-zero.");
            
            double a = (double)values[0], b = (double)values[1], c = (double)values[2];
            return b == 0
                ? 0 
                : c * a / b;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
