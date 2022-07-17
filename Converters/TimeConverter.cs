using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyMusicPlayer
{
    /// <summary>
    /// Сonverting double value to string value and reconverting.
    /// </summary>
    public class TimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts the double value to string value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// String in format "mm:ss".
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan time = TimeSpan.FromSeconds((double)value);

            return time.ToString(@"mm\:ss");
        }

        /// <summary>
        /// Converts the string value to double value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
