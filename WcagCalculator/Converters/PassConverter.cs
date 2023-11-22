using System.Globalization;

namespace WcagCalculator.Converters;

public class PassConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ratio = value as double?;
        var param = parameter as double?;
        if (ratio == null || param == null)
        {
            return Binding.DoNothing;
        }

        return ratio >= param ? "Pass" : "Fail";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}