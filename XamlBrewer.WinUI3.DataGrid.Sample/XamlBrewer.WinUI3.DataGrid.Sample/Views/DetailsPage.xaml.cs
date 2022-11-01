using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using XamlBrewer.WinUI3.DataGrid.Sample.Models;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Views
{
    public sealed partial class DetailsPage : Page
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        public Mountain ViewModel => DataContext as Mountain;
    }
}
