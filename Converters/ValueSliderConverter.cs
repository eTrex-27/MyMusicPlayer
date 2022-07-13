using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MyMusicPlayer
{
    public class ValueSliderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                TimeSpan time = TimeSpan.FromSeconds((double)value);

                return time.ToString(@"mm\:ss");
            }
            catch
            {
                return "00:00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
