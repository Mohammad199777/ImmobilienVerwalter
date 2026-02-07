using System.Globalization;

namespace ImmobilienVerwalter.Maui.Converters;

/// <summary>
/// Returns true if the string is not null or empty
/// </summary>
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrEmpty(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Converts between 1-based month number and 0-based Picker index
/// </summary>
public class MonthIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int month ? month - 1 : 0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int index ? index + 1 : 1;
}
