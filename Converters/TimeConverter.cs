using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyMusicPlayer
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan time = TimeSpan.FromSeconds((double)value);

            return time.ToString(@"mm\:ss");
            /*double valueSlider = (double)value;
            if (valueSlider.ToString().Length > 1)
                return $"00:{valueSlider}";
            else if (valueSlider.ToString().Length == 1)
                return $"00:0{valueSlider}";
            else
                return string.Empty;*/
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
