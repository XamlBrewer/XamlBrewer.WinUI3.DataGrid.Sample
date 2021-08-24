using Microsoft.UI.Xaml;
using XamlBrewer.WinUI3.Services;

namespace XamlBrewer.WinUI3.DataGrid.Sample
{
    public sealed partial class Shell : Window, INavigation
    {
        public Shell()
        {
            Title = "XAML Brewer WinUI 3 DataGrid Sample";

            InitializeComponent();

            Root.RequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Root.RequestedTheme = Root.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
