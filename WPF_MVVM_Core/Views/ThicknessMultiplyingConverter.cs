using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Views.Player
{
    /// <summary>
    /// Вспомогательный класс для анимации списка плейлистов
    /// </summary>
    class ThicknessMultiplyingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
                              CultureInfo culture)
        {
            return new Thickness() { Left = values.Cast<double>().Aggregate((x, y) => x * y) };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
                                    CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
