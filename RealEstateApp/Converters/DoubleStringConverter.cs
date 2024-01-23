using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstateApp.Converters
{
    public class DoubleStringConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return string.Empty;
            
            else
                return string.Format("{0:0.0000}°", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            double? result = null;
            stringValue = stringValue.Remove(stringValue.Length - 1);

            if (double.TryParse(stringValue, out var doubleValue)) result = doubleValue;

            return result;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
