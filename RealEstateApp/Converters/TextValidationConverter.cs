using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Converters
{
    internal class TextValidationConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //return string.IsNullOrWhiteSpace((string)value) ? Color.FromRgb(255, 0, 0) : Color.FromRgb(255, 255, 255)    ;
            return this;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string)value) ? Color.FromRgb(255, 0, 0) : Color.FromRgb(255, 255, 255);

        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;        
        }
    }
}
