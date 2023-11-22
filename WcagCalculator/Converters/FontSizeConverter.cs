using System.Globalization;

namespace WcagCalculator.Converters;

public class FontSizeConverter :IValueConverter
{

    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}