using System;
using System.Globalization;
using System.Windows.Data;

namespace AiCodo
{
    public class ExpressionConverter : IValueConverter
    {
        public string Expression { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Expression.IsNullOrEmpty())
            {
                return value;
            }
            return Expression.Eval("x", value, "p", parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
