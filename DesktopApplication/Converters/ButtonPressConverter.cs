using System;
using System.Windows.Data;

namespace NewsPortal.DesktopApplication.Converters
{
    public class ButtonPressConverter: IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value[1] is int))
                return null;

            var text = (string)value[0];            
            var id =  (int)value[1];

            return new Tuple<String, int>(text, id);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            return  new object[]{ "TEST", 2 };
        }
    }
}
