using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace OfficeDashboard
{
    class StatusToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                string status = (string) value;

                switch (status)
                {
                    case "SUCCESS":
                        return new SolidColorBrush(Colors.Green);

                    case "FAILURE":
                        return new SolidColorBrush(Colors.Red);

                    default:
                        return new SolidColorBrush(Colors.Yellow);
                }
            }

            return new SolidColorBrush(Colors.Yellow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush(Colors.Yellow);
        }
    }
}
