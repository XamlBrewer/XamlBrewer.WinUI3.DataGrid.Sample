using Microsoft.UI.Xaml.Data;
using System;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Services
{
    // Just for information. Not used anymore.

    public class SliderValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            return $"{value} m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
