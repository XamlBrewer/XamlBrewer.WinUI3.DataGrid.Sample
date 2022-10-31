using Microsoft.UI.Xaml;
using XamlBrewer.WinUI3.Services;

namespace XamlBrewer.WinUI3.DataGrid.Sample
{
    public partial class App : Application
    {
        private Shell shell;

        public App()
        {
            InitializeComponent();
        }

        public INavigation Navigation => shell;

        public string Title => shell.Title;

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            shell = new Shell();
            shell.Activate();
        }
    }
}
