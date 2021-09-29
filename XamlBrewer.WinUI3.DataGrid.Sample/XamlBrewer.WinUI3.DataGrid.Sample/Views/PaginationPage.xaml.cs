using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using XamlBrewer.WinUI3.DataGrid.Sample.ViewModels;

namespace XamlBrewer.WinUI3.DataGrid.Sample.Views
{
    public sealed partial class PaginationPage : Page
    {
        public PaginationPage()
        {
            InitializeComponent();
            Loaded += PaginationPage_Loaded;
        }

        private async void PaginationPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private PaginationPageViewModel ViewModel => DataContext as PaginationPageViewModel;
    }
}
