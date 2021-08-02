using Microsoft.UI.Xaml.Data;
using System;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Services
{
    public class SliderValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            return String.Format($"{value} m");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
