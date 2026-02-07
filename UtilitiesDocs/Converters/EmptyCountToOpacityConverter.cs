using Microsoft.UI.Xaml.Data;
using System;

namespace UtilitiesDocs.Converters
{
    public class EmptyCountToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int count)
            {
                return count == 0 ? 1.0 : 0.0;
            }
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
