using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;
using XamlBrewer.WinUI3.Services;

namespace XamlBrewer.WinUI3.DataGrid.Sample
{
    public sealed partial class Shell : Window, INavigation
    {
        public Shell()
        {
            Title = "XAML Brewer WinUI 3 DataGrid Sample";

            InitializeComponent();

            var appWindow = this.GetAppWindow();
            appWindow.SetIcon("Assets/Beer.ico");

            Root.RequestedTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Root.RequestedTheme = Root.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
